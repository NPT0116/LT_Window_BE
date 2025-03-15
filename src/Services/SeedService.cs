using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using src.Data;
using src.Models;

namespace src.Services
{
    // Lớp chứa cấu trúc dữ liệu seed được định nghĩa trong file seed.json
    public class SeedData
    {
        public List<Manufacturer> Manufacturers { get; set; } = new List<Manufacturer>();
        public List<ItemGroup> ItemGroups { get; set; } = new List<ItemGroup>();
        public List<Customer> Customers { get; set; } = new List<Customer>();
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
            // Xác định đường dẫn tới file seed.json
            var seedFile = Path.Combine(_env.ContentRootPath, "seed.json");
            if (!File.Exists(seedFile))
            {
                // Nếu không tìm thấy file, có thể log thông báo và thoát
                Console.WriteLine("Seed file not found: " + seedFile);
                return;
            }

            // Đọc nội dung file seed.json
            var json = await File.ReadAllTextAsync(seedFile);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var seedData = JsonSerializer.Deserialize<SeedData>(json, options);

            if (seedData == null)
            {
                Console.WriteLine("Không thể parse dữ liệu seed từ file JSON.");
                return;
            }

            // Seed Manufacturers nếu bảng rỗng
            if (!_context.Manufacturers.Any())
            {
                _context.Manufacturers.AddRange(seedData.Manufacturers);
            }

            // Seed ItemGroups (bao gồm Items, Colors, Variants) nếu bảng rỗng
            if (!_context.ItemGroups.Any())
            {
                _context.ItemGroups.AddRange(seedData.ItemGroups);
            }

            // Seed Customers (bao gồm Invoices và InvoiceDetails) nếu bảng rỗng
            if (!_context.Customers.Any())
            {
                _context.Customers.AddRange(seedData.Customers);
            }

            await _context.SaveChangesAsync();
        }
    }
}
