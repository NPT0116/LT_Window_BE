using System;

namespace src.Exceptions.InventoryTransaction
{
    /// <summary>
    /// Exception được ném ra khi cố gắng xóa một giao dịch tồn kho mà không phải là giao dịch mới nhất.
    /// Chỉ cho phép xóa giao dịch mới nhất để đảm bảo tính nhất quán của dữ liệu tồn kho.
    /// </summary>
    public class InventoryTransactionDeleteBlockDueToExistsOtherTransactionAbove : BaseException
    {

        public InventoryTransactionDeleteBlockDueToExistsOtherTransactionAbove(Guid transactionId)
            : base($"Không thể xóa giao dịch với ID {transactionId} vì đã có giao dịch mới hơn được ghi nhận. Chỉ cho phép xóa giao dịch mới nhất.", System.Net.HttpStatusCode.BadRequest)
        {
        }



    }
}
