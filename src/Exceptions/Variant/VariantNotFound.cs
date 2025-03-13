using System;

namespace src.Exceptions.Variant;

public class VariantNotFound: BaseException
{
    public VariantNotFound(Guid id) : base($"Variant with id {id} not found", System.Net.HttpStatusCode.NotFound)
    {
    }
}
