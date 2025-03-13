using System;

namespace src.Dto.Item;

public class UpdateItemDto
{
    public Guid ItemId { get; set; }
    public  Guid? ItemGroupId { get; set; }
    public  string ItemName { get; set; }
    public  string? Description { get; set; }
    public  string? Picture { get; set; }
    public  DateTime ReleaseDate { get; set; }
    public  Guid ManufacturerId { get; set; }
}
