using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using src.Dto.InventoryTransaction;
using src.Interfaces;
using src.Dto.Variant;
using src.Utils;

namespace src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryTransactionController : ControllerBase
    {
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;

        public InventoryTransactionController(IInventoryTransactionRepository inventoryTransactionRepository)
        {
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        /// <summary>
        /// Endpoint tạo giao dịch Inbound cho một Variant.
        /// </summary>
        /// <param name="request">Request body bao gồm VariantId và số lượng nhập</param>
        /// <returns>Trả về VariantDto đã được cập nhật tồn kho</returns>
        [HttpPost("inbound")]
        public async Task<IActionResult> CreateInboundTransaction([FromBody] CreateInboundTransactionRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null.");

            var updatedVariant = await _inventoryTransactionRepository.CreateInBoundTransaction(
                request.VariantId,
                request.Quantity
            );

            return Ok(new Response<VariantDto>(updatedVariant));
        }

        /// <summary>
        /// Endpoint lấy danh sách giao dịch cho một Variant theo ID.
        /// </summary>
        /// <param name="variantId">ID của Variant</param>
        /// <returns>Danh sách InventoryTransactionDto</returns>
        [HttpGet("{variantId:guid}")]
        public async Task<IActionResult> GetTransactionsByVariantId(Guid variantId)
        {
            var transactions = await _inventoryTransactionRepository.GetInventoryTransactionsByVariantId(variantId);
            return Ok(new Response<ICollection<InventoryTransactionDto>>(transactions));
        }
    

        [HttpDelete("{transactionId:guid}")]
        public async Task<IActionResult> DeleteTransaction(Guid transactionId)
        {
            await _inventoryTransactionRepository.DeleteTransaction(transactionId);
            return Ok();
        }

}
}