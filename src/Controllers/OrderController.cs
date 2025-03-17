using Microsoft.AspNetCore.Mvc;
using src.Dto;
using src.Dto.Order;
using src.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace src.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Tạo phiếu đặt hàng mới
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            var order = await _orderRepository.CreateOrderAsync(orderDto);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderID }, order);
        }

        // Lấy danh sách tất cả đơn hàng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return Ok(orders);
        }

        // Lấy 1 đơn hàng theo ID
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return Ok(order);
        }

        // Cập nhật đơn hàng
        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody] OrderDto orderDto)
        {
            if (orderId != orderDto.OrderID)
                return BadRequest("Mã đơn hàng không khớp");

            await _orderRepository.UpdateOrderAsync(orderDto);
            return NoContent();
        }

        // Xóa đơn hàng
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            await _orderRepository.DeleteOrderAsync(orderId);
            return NoContent();
        }
    }
}
