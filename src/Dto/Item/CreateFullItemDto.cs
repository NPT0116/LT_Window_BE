using System;

namespace src.Dto.Item;
public class ColorCreateDto
{
    // Trường tạm để liên kết giữa color và variant (do FE tự sinh, ví dụ: 1, 2, 3)
    public int TempId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UrlImage { get; set; } = string.Empty;
    // Không cần ColorId từ FE (hoặc FE có thể gửi Guid.Empty)
    public Guid ColorId { get; set; }
}

public class VariantCreateDto
{
    public string Storage { get; set; } = string.Empty;
    public float CostPrice { get; set; }
    public float SellingPrice { get; set; }
    public int StockQuantity { get; set; }
    // Sử dụng trường tạm để liên kết với color đã tạo
    public int ColorTempId { get; set; }
}


public class CreateFullItemDto
{
    public ItemCreateDto Item { get; set; }
    public List<ColorCreateDto> Colors { get; set; } = new();
    public List<VariantCreateDto> Variants { get; set; } = new();
}
