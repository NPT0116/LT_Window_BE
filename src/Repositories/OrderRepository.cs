using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto;
using src.Dto.Order;
using src.Exceptions.Customer;
using src.Exceptions.Order;
using src.Interfaces;
using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        
        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tạo phiếu đặt hàng mới
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));

            if (orderDto.OrderDetailDtos == null || !orderDto.OrderDetailDtos.Any())
                throw new OrderMustHaveAtLestOneDetail();

            // Kiểm tra sự tồn tại của khách hàng
            var customer = await _context.Customers.FindAsync(orderDto.CustomerID);
            if (customer == null)
                throw new CustomerNotFound(orderDto.CustomerID);

            // Tạo mới Order
            var order = new Order
            {
                OrderID = Guid.NewGuid(),
                CustomerID = orderDto.CustomerID,
                OrderDate = orderDto.OrderDate,
                ExpectedDeliveryDate = orderDto.ExpectedDeliveryDate,
                Status = orderDto.Status, // ví dụ: OrderStatus.Created
                TotalAmount = orderDto.TotalAmount
            };

            // Tạo danh sách chi tiết đơn hàng
            foreach (var detailDto in orderDto.OrderDetailDtos)
            {
                var orderDetail = new OrderDetail
                {
                    OrderDetailID = Guid.NewGuid(),
                    OrderID = order.OrderID,
                    VariantID = detailDto.VariantID,
                    Quantity = detailDto.Quantity,
                    Price = detailDto.Price
                };
                order.OrderDetails.Add(orderDetail);
            }

            // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            // Mapping Order -> OrderDto
            var orderResult = new OrderDto
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailID = od.OrderDetailID,
                    OrderID = od.OrderID,
                    VariantID = od.VariantID,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };

            return orderResult;
        }

        // Lấy 1 đơn hàng theo ID
        public async Task<OrderDto> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
            if (order == null)
                throw new OrderNotFound(orderId);

            return new OrderDto
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailID = od.OrderDetailID,
                    OrderID = od.OrderID,
                    VariantID = od.VariantID,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };
        }

        // Lấy danh sách tất cả các đơn hàng
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailID = od.OrderDetailID,
                    OrderID = od.OrderID,
                    VariantID = od.VariantID,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            });
        }

        // Cập nhật thông tin đơn hàng
        public async Task UpdateOrderAsync(OrderDto orderDto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == orderDto.OrderID);
            if (order == null)
                throw new Exception($"Không tìm thấy đơn hàng với mã {orderDto.OrderID}");

            // Cập nhật các thông tin của đơn hàng (các trường cần update tùy theo yêu cầu)
            order.OrderDate = orderDto.OrderDate;
            order.ExpectedDeliveryDate = orderDto.ExpectedDeliveryDate;
            order.Status = orderDto.Status;
            order.TotalAmount = orderDto.TotalAmount;
            // Nếu cần update các OrderDetail thì có thể xử lý thêm tại đây

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // Xóa đơn hàng
        public async Task DeleteOrderAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
            if (order == null)
                throw new Exception($"Không tìm thấy đơn hàng với mã {orderId}");

            // Xóa các chi tiết đơn hàng trước (nếu có)
            _context.RemoveRange(order.OrderDetails);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
