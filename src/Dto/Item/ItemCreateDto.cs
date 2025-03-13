using System;
using System.Security.AccessControl;

namespace src.Dto.Item;

public class ItemCreateDto
{
    public  Guid? ItemGroupId { get; set; }
    public  string ItemName { get; set; }
    public  string? Description { get; set; }
    public  string? Picture { get; set; }
    public  DateTime ReleaseDate { get; set; }
    public  Guid ManufacturerId { get; set; }
}   
