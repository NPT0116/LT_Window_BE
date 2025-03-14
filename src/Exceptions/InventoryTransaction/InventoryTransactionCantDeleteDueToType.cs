using System;

namespace src.Exceptions.InventoryTransaction;

public class InventoryTransactionCantDeleteDueToType: BaseException
{
    public InventoryTransactionCantDeleteDueToType() : base("Không thể xóa giao dịch xuất hàng (Outbound). Giao dịch xuất hàng sẽ được xóa khi xóa hóa đơn.", System.Net.HttpStatusCode.BadRequest)
    {
    }
}
