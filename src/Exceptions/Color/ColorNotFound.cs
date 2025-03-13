using System;

namespace src.Exceptions.Color;

public class ColorNotFound: BaseException
{
    public ColorNotFound(Guid id) : base($"Color with id {id} was not found", System.Net.HttpStatusCode.NotFound)
    {
    }
}

