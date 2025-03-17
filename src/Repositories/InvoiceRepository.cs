using System;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto;
using src.Dto.Invoice;
using src.Exceptions.Customer;
using src.Exceptions.Invoice;
using src.Exceptions.Variant;
using src.Interfaces;
using src.Models;
using src.Query;
using src.Utils;

namespace src.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;
    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto invoiceDto)
{
    // Kiểm tra đầu vào không null và danh sách invoice details không rỗng
    if (invoiceDto == null)
        throw new ArgumentNullException(nameof(invoiceDto));

    if (invoiceDto.invoiceDetailDtos == null || !invoiceDto.invoiceDetailDtos.Any())
        throw new ArgumentException("At least one invoice detail is required.", nameof(invoiceDto.invoiceDetailDtos));

    // Tính tổng tiền kỳ vọng từ các invoice detail
    float expectedTotal = invoiceDto.invoiceDetailDtos.Sum(detail => detail.Price * detail.Quantity);
    if (Math.Abs(invoiceDto.TotalAmount - expectedTotal) > 0.01)
        throw new TotalAmountNotMatchSumInvoiceDetail();

    // Kiểm tra sự tồn tại của Customer
    var customer = await _context.Customers.FindAsync(invoiceDto.CustomerID);
    if (customer == null)
        throw new CustomerNotFound(invoiceDto.CustomerID);

    // Lấy danh sách các variant (không trùng lặp) được đề cập trong invoice details
    var variantIds = invoiceDto.invoiceDetailDtos.Select(d => d.VariantID).Distinct();
    var variantDict = new Dictionary<Guid, Variant>();

    foreach (var variantId in variantIds)
    {
        // Kiểm tra Variant tồn tại và lấy Variant từ cơ sở dữ liệu
        var variant = await _context.Variants.FindAsync(variantId);
        if (variant == null)
            throw new VariantNotFound(variantId);

        variantDict[variantId] = variant;
    }

    // Tạo mới Invoice
    var invoice = new Invoice
    {
        InvoiceID = Guid.NewGuid(),
        CustomerID = invoiceDto.CustomerID,
        Date = invoiceDto.Date,
        TotalAmount = invoiceDto.TotalAmount
    };

    // Chuẩn bị danh sách invoice details và các giao dịch tồn kho
    var invoiceDetailList = new List<InvoiceDetail>();
    var inventoryTransactionList = new List<InventoryTransaction>();
    var currentUtc = DateTime.UtcNow;

    // Lặp qua từng invoice detail
    foreach (var detailDto in invoiceDto.invoiceDetailDtos)
    {
        // Lấy Variant tương ứng từ dictionary
        var variant = variantDict[detailDto.VariantID];
        if (variant.StockQuantity < detailDto.Quantity)
            throw new VariantInsufficientStocklQuantity(detailDto.VariantID);
        // Tạo mới InvoiceDetail
        var invoiceDetail = new InvoiceDetail
        {
            InvoiceDetailID = Guid.NewGuid(),
            InvoiceID = invoice.InvoiceID,
            VariantID = detailDto.VariantID,
            Quantity = detailDto.Quantity,
            Price = detailDto.Price
        };

        invoiceDetailList.Add(invoiceDetail);
        invoice.InvoiceDetails.Add(invoiceDetail);

        // Tạo giao dịch xuất kho (Outbound) cho chi tiết này
        var inventoryTransaction = new InventoryTransaction
        {
            TransactionID = Guid.NewGuid(),
            VariantID = detailDto.VariantID,
            TransactionType = InventoryTransactionType.Outbound,
            Quantity = detailDto.Quantity,
            TransactionDate = currentUtc,
            InvoiceDetailID = invoiceDetail.InvoiceDetailID
        };

        inventoryTransactionList.Add(inventoryTransaction);

        // Cập nhật trực tiếp tồn kho trong Variant
        variant.StockQuantity -= detailDto.Quantity;
    }

    // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
    using var dbTransaction = await _context.Database.BeginTransactionAsync();
    try
    {
        await _context.Invoices.AddAsync(invoice);
        await _context.InvoiceDetails.AddRangeAsync(invoiceDetailList);
        await _context.InventoryTransactions.AddRangeAsync(inventoryTransactionList);

        // Cập nhật các Variant đã được thay đổi
        foreach (var variant in variantDict.Values)
        {
            _context.Variants.Update(variant);
        }

        await _context.SaveChangesAsync();
        await dbTransaction.CommitAsync();
    }
    catch (Exception)
    {
        await dbTransaction.RollbackAsync();
        throw;
    }

    // Chuyển đổi kết quả sang InvoiceDto trả về cho FE
    var result = new InvoiceDto
    {
        InvoiceID = invoice.InvoiceID,
        CustomerID = invoice.CustomerID,
        Date = invoice.Date,
        TotalAmount = invoice.TotalAmount,
        InvoiceDetails = [.. invoiceDetailList.Select(detail => new InvoiceDetailDto
        {
            InvoiceDetailID = detail.InvoiceDetailID,
            VariantID = detail.VariantID,
            Quantity = detail.Quantity,
            Price = detail.Price
        })]
    };

    return result;
}



   public async Task<PagedResponse<ICollection<InvoiceDto>>> GetAllInvoicesAsync(InvoiceQueryParameter queryInvoiceParameter)
{
    // Lấy query ban đầu từ bảng Invoices
    var query = _context.Invoices
        .Include(i => i.InvoiceDetails)
        .AsQueryable();

    // Áp dụng các điều kiện lọc nếu có
    if (queryInvoiceParameter.invoiceDatetimeQueryParameter != null)
    {
        query = query.Where(i => i.Date >= queryInvoiceParameter.invoiceDatetimeQueryParameter.FromDate &&
                                 i.Date <= queryInvoiceParameter.invoiceDatetimeQueryParameter.ToDate);
        query = queryInvoiceParameter.invoiceDatetimeQueryParameter.sortDirection == "desc"
            ? query.OrderByDescending(i => i.Date)
            : query.OrderBy(i => i.Date);
    }
    if (!string.IsNullOrWhiteSpace(queryInvoiceParameter.CustomerName))
    {
        query = query.Where(i => i.Customer.Name.Contains(queryInvoiceParameter.CustomerName));
    }
    if (!string.IsNullOrWhiteSpace(queryInvoiceParameter.CustomerPhone))
    {
        query = query.Where(i => i.Customer.Phone.Contains(queryInvoiceParameter.CustomerPhone));
    }

    // Lấy tổng số record
    int totalRecords = await query.CountAsync();
    // Tính số trang (có thể làm tròn lên nếu có dư)
    int totalPages = (int)Math.Ceiling(totalRecords / (double)queryInvoiceParameter.PageSize);

    // Lấy danh sách theo phân trang
    var finalResponse = await query
        .Skip((queryInvoiceParameter.PageNumber - 1) * queryInvoiceParameter.PageSize)
        .Take(queryInvoiceParameter.PageSize)
        .ToListAsync();

    var invoiceDtos = finalResponse.Select(i => new InvoiceDto
    {
        InvoiceID = i.InvoiceID,
        CustomerID = i.CustomerID,
        Date = i.Date,
        TotalAmount = i.TotalAmount,
        InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDto
        {
            InvoiceDetailID = d.InvoiceDetailID,
            VariantID = d.VariantID,
            Quantity = d.Quantity,
            Price = d.Price
        }).ToList()
    }).ToList();

    // Tạo đối tượng phân trang với các trường bổ sung TotalRecords và TotalPages
    var response = new PagedResponse<ICollection<InvoiceDto>>(invoiceDtos, queryInvoiceParameter.PageNumber, queryInvoiceParameter.PageSize)
    {
        TotalRecords = totalRecords,
        TotalPages = totalPages
    };

    return response;
}



    public async Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId)
    {
        var invoice = await _context.Invoices.Include(i => i.InvoiceDetails).FirstOrDefaultAsync(i => i.InvoiceID == invoiceId);
        if (invoice == null)
            throw new InvoiceNotFound(invoiceId);
        var response = new InvoiceDto
        {
            InvoiceID = invoice.InvoiceID,
            CustomerID = invoice.CustomerID,
            Date = invoice.Date,
            TotalAmount = invoice.TotalAmount,
            InvoiceDetails = [.. invoice.InvoiceDetails.Select(d => new InvoiceDetailDto
            {
                InvoiceDetailID = d.InvoiceDetailID,
                VariantID = d.VariantID,
                Quantity = d.Quantity,
                Price = d.Price
            })]
        };
        return response;
    }

    public async Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerIdAsync(Guid customerId)
    {
        var customerInvoices = await _context.Invoices.Include(i => i.InvoiceDetails).Where(i => i.CustomerID == customerId).ToListAsync();
        var invoiceDtos = customerInvoices.Select(i => new InvoiceDto
        {
            InvoiceID = i.InvoiceID,
            CustomerID = i.CustomerID,
            Date = i.Date,
            TotalAmount = i.TotalAmount,
            InvoiceDetails = [.. i.InvoiceDetails.Select(d => new InvoiceDetailDto
            {
                InvoiceDetailID = d.InvoiceDetailID,
                VariantID = d.VariantID,
                Quantity = d.Quantity,
                Price = d.Price
            })]
        });
        return invoiceDtos;
    }
}
