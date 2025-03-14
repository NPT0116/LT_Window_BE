using System;

namespace src.Exceptions.Variant;

public class VariantExistsTransactionOrInvoice: BaseException
{
    public VariantExistsTransactionOrInvoice(Guid variantId) : base($"Không thể xóa Variant có id: {variantId} vì đã được sử dụng trong hóa đơn hoặc giao dịch tồn kho.", System.Net.HttpStatusCode.BadRequest)
    {
    }
}
