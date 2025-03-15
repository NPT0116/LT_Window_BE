using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using src.Data;
using src.Models;
using Microsoft.EntityFrameworkCore;

namespace src.Services
{
    // Lớp chứa cấu trúc dữ liệu seed từ file JSON
    public class SeedData
    {
        public List<Manufacturer> Manufacturers { get; set; } = new List<Manufacturer>();
        public List<ItemGroup> ItemGroups { get; set; } = new List<ItemGroup>();
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
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

        /// <summary>
        /// Seed dữ liệu từ file JSON.
        /// Nếu resetDb = true, DB sẽ được xóa sạch và migration được chạy lại.
        /// </summary>
        public async Task SeedAsync(bool resetDb = false)
        {
            // 1. Xóa và chạy lại migration nếu có yêu cầu reset
            if (resetDb)
            {
                Console.WriteLine("Resetting database...");
                await _context.Database.EnsureDeletedAsync();   // Xóa DB
                await _context.Database.MigrateAsync();         // Tạo lại DB theo migration
            }

            // 2. Đọc file seed JSON
            var seedFile = Path.Combine(_env.ContentRootPath, "seed.json");
            if (!File.Exists(seedFile))
            {
                Console.WriteLine("Seed file not found: " + seedFile);
                return;
            }

            var json = await File.ReadAllTextAsync(seedFile);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var seedData = JsonSerializer.Deserialize<SeedData>(json, options);

            if (seedData == null)
            {
                Console.WriteLine("Unable to parse seed data from JSON.");
                return;
            }

            // 3. Seed Manufacturers nếu bảng trống
            if (!_context.Manufacturers.Any())
            {
                _context.Manufacturers.AddRange(seedData.Manufacturers);
            }

            // 4. Seed ItemGroups (bao gồm Items, Colors, Variants) nếu bảng trống
            if (!_context.ItemGroups.Any())
            {
                _context.ItemGroups.AddRange(seedData.ItemGroups);
            }

            // 5. Seed Customers (bao gồm Invoices, InvoiceDetails, InventoryTransaction)
            //    nếu bảng Customers trống
            if (!_context.Customers.Any())
            {
                // Khi AddRange Customers, EF sẽ track toàn bộ object graph:
                //  => Invoice, InvoiceDetail, InventoryTransaction (nếu có)
                _context.Customers.AddRange(seedData.Customers);
            }
            if (!_context.InventoryTransactions.Any())
            {
                _context.InventoryTransactions.AddRange(seedData.InventoryTransactions);
            }
            // 6. Lưu các thay đổi
            await _context.SaveChangesAsync();
        }
    }
}
