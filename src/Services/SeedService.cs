using System;
using System.Text.Json;
using src.Data;
using src.Models;

namespace src.Services;

public class SeedData
{
    public List<Manufacturer> Manufacturers { get; set; } = new List<Manufacturer>();
    public List<ItemGroup> ItemGroups { get; set; } = new List<ItemGroup>();
}

    public class SeedService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _env;

        public SeedService(ApplicationDbContext context, IHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
public async Task SeedAsync()
{
    // Seed Manufacturers nếu chưa có
    if (!_context.Manufacturers.Any())
    {
        var seedFile = Path.Combine(_env.ContentRootPath, "seed.json");
        if (File.Exists(seedFile))
        {
            var json = await File.ReadAllTextAsync(seedFile);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var seedData = JsonSerializer.Deserialize<SeedData>(json, options);
            if (seedData != null && seedData.Manufacturers != null)
            {
                foreach (var m in seedData.Manufacturers)
                {
                    // Thêm kiểm tra nếu cần: ví dụ, kiểm tra xem manufacturer có tồn tại không
                    _context.Manufacturers.Add(m);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
    
    // Seed ItemGroups (và Items) nếu chưa có
    if (!_context.ItemGroups.Any())
    {
        var seedFile = Path.Combine(_env.ContentRootPath, "seed.json");
        if (File.Exists(seedFile))
        {
            var json = await File.ReadAllTextAsync(seedFile);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var seedData = JsonSerializer.Deserialize<SeedData>(json, options);
            if (seedData != null)
            {
                foreach (var group in seedData.ItemGroups)
                {
                    if (group.ItemGroupID == Guid.Empty)
                        group.ItemGroupID = Guid.NewGuid();
                    
                    foreach (var item in group.Items)
                    {
                        if (item.ItemID == Guid.Empty)
                            item.ItemID = Guid.NewGuid();

                        // Chuyển đổi ReleaseDate sang UTC nếu cần
                        if (item.ReleaseDate.Kind != DateTimeKind.Utc)
                        {
                            item.ReleaseDate = DateTime.SpecifyKind(item.ReleaseDate, DateTimeKind.Utc);
                        }
                        
                        // Nếu ManufacturerID là Guid.Empty, bạn có thể gán mặc định cho nó
                        // Nhưng nếu đã có trong file seed.json, hãy đảm bảo nó trùng với manufacturer "Apple"
                        if (item.ManufacturerID == Guid.Empty)
                        {
                            // Gán ManufacturerID của "Apple" đã seed, ví dụ:
                            item.ManufacturerID = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
                        }
                        
                        // Gán ItemID cho các Color của item
                        if (item.Colors != null)
                        {
                            foreach (var color in item.Colors)
                            {
                                color.ItemID = item.ItemID;
                            }
                        }
                        
                        // Gán ItemID cho các Variant của item
                        if (item.Variants != null)
                        {
                            foreach (var variant in item.Variants)
                            {
                                variant.ItemID = item.ItemID;
                            }
                        }
                    }
                    _context.ItemGroups.Add(group);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}

    
    


    }