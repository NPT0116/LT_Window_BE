using System;

namespace src.Exceptions.Variant;

public class VariantInsufficientStocklQuantity: BaseException
{
    public VariantInsufficientStocklQuantity(Guid variantId) : base($"Variant {variantId} has insufficient stock quantity", System.Net.HttpStatusCode.BadRequest)
    {
    }
}
