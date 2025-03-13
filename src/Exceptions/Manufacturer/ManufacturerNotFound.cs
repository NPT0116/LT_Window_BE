using System;

namespace src.Exceptions.Manufacturer;

public class ManufacturerNotFound: BaseException
{
    public ManufacturerNotFound(Guid id) : base($"Manufacturer with id {id} was not found", System.Net.HttpStatusCode.NotFound)
    {
    }
}
