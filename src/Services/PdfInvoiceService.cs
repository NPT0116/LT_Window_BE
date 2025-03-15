using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using src.Dto.Invoice;
using src.Interfaces;

namespace src.Services
{
    public class PdfInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IVariantRepository _variantRepository;
        private readonly IColorRepository _colorRepository;

        public PdfInvoiceService(
            IInvoiceRepository invoiceRepository,
            ICustomerRepository customerRepository,
            IVariantRepository variantRepository,
            IColorRepository colorRepository)
        {
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _variantRepository = variantRepository;
            _colorRepository = colorRepository;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId)
        {
            // 1. Lấy dữ liệu hóa đơn
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
                throw new Exception($"Không tìm thấy hóa đơn với ID: {invoiceId}");

            // 2. Lấy thông tin khách hàng
            var customer = await _customerRepository.GetCustomerByIdAsync(invoice.CustomerID);

            // 3. Chuẩn bị danh sách chi tiết hóa đơn (bao gồm ảnh sản phẩm)
            var detailInfos = new List<InvoiceDetailInfo>();
            float totalAmount = 0;
            for (int i = 0; i < invoice.InvoiceDetails.Count; i++)
            {
                var detail = invoice.InvoiceDetails[i];
                var variantName = await _variantRepository.GetVariantNameByIdAsync(detail.VariantID);
                var imageUrl = await _colorRepository.GetImageByVariantIdAsync(detail.VariantID);
                var imageBytes = await DownloadImageAsync(imageUrl);

                float lineTotal = detail.Price * detail.Quantity;
                totalAmount += lineTotal;

                detailInfos.Add(new InvoiceDetailInfo
                {
                    Index = i + 1,
                    VariantName = variantName,
                    Quantity = detail.Quantity,
                    Price = detail.Price,
                    LineTotal = lineTotal,
                    ImageBytes = imageBytes
                });
            }

            // 4. Tạo PDF bằng QuestPDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content().Element(outer =>
                    {
                        // Khung ngoài
                        outer.Border(1)
                             .BorderColor(Colors.Grey.Medium)
                             .Padding(20)
                             .Column(column =>
                        {
                            column.Spacing(10);

                            // Header: tiêu đề và thông tin công ty
                            column.Item().Element(e => e.AlignCenter().Column(col =>
                            {
                                col.Item().Text("HÓA ĐƠN MUA HÀNG")
                                    .FontSize(18).SemiBold().FontColor(Colors.Blue.Medium);
                                col.Item().Text("Công ty Điện thoại Minh Tuấn");
                                col.Item().Text("Địa chỉ: 123 Lê Lợi, Q.1, TP.HCM");
                                col.Item().Text("Điện thoại: 0123 456 789");
                            }));

                            // Đường kẻ ngang
                            column.Item().Row(row =>
                            {
                                row.RelativeColumn().Height(1).Background(Colors.Grey.Lighten2);
                            });

                            // Thông tin hóa đơn & khách hàng
                            column.Item().Text($"Ngày lập: {invoice.Date:dd/MM/yyyy}");
                            column.Item().Text($"Mã HĐ: {invoice.InvoiceID}");
                            column.Item().Text($"Khách hàng: {customer.Name}");
                            column.Item().Text($"Email: {customer.Email}");
                            column.Item().Text($"Điện thoại: {customer.Phone}");
                            column.Item().Text($"Địa chỉ: {customer.Address}");

                            // Tiêu đề bảng chi tiết
                            column.Item().Text("Chi tiết hóa đơn:").SemiBold();

                            // Bảng chi tiết (không có đường viền riêng cho từng ô)
                            column.Item().Element(tableContainer =>
                            {
                                tableContainer.Table(table =>
                                {
                                    // Định nghĩa cột
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(40);  // STT
                                        columns.ConstantColumn(60);  // Ảnh
                                        columns.RelativeColumn(2);   // Sản phẩm
                                        columns.ConstantColumn(40);  // SL
                                        columns.ConstantColumn(60);  // Đơn giá
                                        columns.ConstantColumn(60);  // Thành tiền
                                    });

                                    // Header bảng
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(x => HeaderCellStyle(x)).Text("STT");
                                        header.Cell().Element(x => HeaderCellStyle(x)).Text("Ảnh");
                                        header.Cell().Element(x => HeaderCellStyle(x)).Text("Sản phẩm");
                                        header.Cell().Element(x => HeaderCellStyle(x)).Text("SL");
                                        header.Cell().Element(x => HeaderCellStyle(x)).Text("Đơn giá");
                                        header.Cell().Element(x => HeaderCellStyle(x)).Text("Thành tiền");
                                    });

                                    // Dữ liệu bảng
                                    foreach (var info in detailInfos)
                                    {
                                        table.Cell().Element(x => DataCellStyle(x)).Text(info.Index.ToString());
                                        table.Cell().Element(x => DataCellStyle(x)).Element(imgCol =>
                                        {
                                            if (info.ImageBytes != null && info.ImageBytes.Length > 0)
                                                imgCol.Image(info.ImageBytes, ImageScaling.FitWidth);
                                            else
                                                imgCol.Text("(No Image)").FontColor(Colors.Grey.Medium);
                                        });
                                        table.Cell().Element(x => DataCellStyle(x)).Text(info.VariantName);
                                        table.Cell().Element(x => DataCellStyle(x)).AlignRight().Text(info.Quantity.ToString());
                                        table.Cell().Element(x => DataCellStyle(x)).AlignRight().Text(info.Price.ToString("N0"));
                                        table.Cell().Element(x => DataCellStyle(x)).AlignRight().Text(info.LineTotal.ToString("N0"));
                                    }
                                });
                            });

                            // Tổng tiền
                            column.Item().AlignRight().Text($"Tổng tiền: {totalAmount:N0} VNĐ")
                                .FontSize(14).SemiBold();

                            // Khoảng trống
                            column.Item().Height(20);

                            // Phần ký tên
                            column.Item().Row(row =>
                            {
                                row.RelativeColumn().AlignCenter().Column(col2 =>
                                {
                                    col2.Spacing(5);
                                    col2.Item().Text("Người mua hàng").SemiBold();
                                    col2.Item().Text("(Ký, ghi rõ họ tên)").FontSize(10).Italic();
                                });
                                row.RelativeColumn().AlignCenter().Column(col2 =>
                                {
                                    col2.Spacing(5);
                                    col2.Item().Text("Người bán hàng").SemiBold();
                                    col2.Item().Text("(Ký, ghi rõ họ tên)").FontSize(10).Italic();
                                });
                            });
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        /// <summary>
        /// Tải ảnh từ URL, trả về mảng byte. Nếu lỗi, trả về mảng rỗng.
        /// </summary>
        private async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return Array.Empty<byte>();

            try
            {
                using var httpClient = new HttpClient();
                return await httpClient.GetByteArrayAsync(imageUrl);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Lớp chứa thông tin chi tiết cho mỗi dòng bảng.
        /// </summary>
        private class InvoiceDetailInfo
        {
            public int Index { get; set; }
            public string VariantName { get; set; }
            public int Quantity { get; set; }
            public float Price { get; set; }
            public float LineTotal { get; set; }
            public byte[] ImageBytes { get; set; }
        }

        /// <summary>
        /// Style cho ô header của bảng (không viền riêng, chỉ padding và căn giữa).
        /// </summary>
        private IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Padding(5)
                .Background(Colors.Grey.Lighten3)
                .AlignCenter()
                .AlignMiddle();
        }

        /// <summary>
        /// Style cho ô dữ liệu của bảng (không viền riêng, chỉ padding và căn giữa).
        /// </summary>
        private IContainer DataCellStyle(IContainer container)
        {
            return container
                .Padding(5)
                .AlignCenter()
                .AlignMiddle();
        }
    }
}
