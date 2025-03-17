using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using src.Dto;
using src.Dto.Order;

namespace src.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto);
        Task<OrderDto> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task UpdateOrderAsync(OrderDto orderDto);
        Task DeleteOrderAsync(Guid orderId);
    }
}
