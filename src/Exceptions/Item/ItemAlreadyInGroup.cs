using System;

namespace src.Exceptions.Item;

public class ItemAlreadyInGroup: BaseException
{
    public ItemAlreadyInGroup(Guid itemID, Guid itemGroupID) : base($"Item with ID {itemID} already in group with ID {itemGroupID}", System.Net.HttpStatusCode.BadRequest)
    {
    }
}
