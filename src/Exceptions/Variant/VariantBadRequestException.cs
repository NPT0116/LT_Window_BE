using System;

namespace src.Exceptions.Variant;

public class VariantBadRequestException: BaseException
{
    public VariantBadRequestException(string message) : base(message, System.Net.HttpStatusCode.BadRequest)
    {
    }
}
