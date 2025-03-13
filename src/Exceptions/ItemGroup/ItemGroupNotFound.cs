using System;

namespace src.Exceptions.ItemGroup;

public class ItemGroupNotFound: BaseException
{
    public ItemGroupNotFound(Guid itemGroupID) : base($"ItemGroup with ID {itemGroupID} not found", System.Net.HttpStatusCode.NotFound)
    {
    }
}
