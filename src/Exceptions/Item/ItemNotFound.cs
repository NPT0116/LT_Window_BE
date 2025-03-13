using System;

namespace src.Exceptions.Item;

public class ItemNotFound: BaseException
{
    public ItemNotFound(Guid itemID) : base($"Item with ID {itemID} not found", System.Net.HttpStatusCode.NotFound)
    {
    }
}
