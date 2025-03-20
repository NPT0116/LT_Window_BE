using System;

namespace src.Query;

public class ItemGroupQueryParameter
{
    public string ItemGroupName { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
}
