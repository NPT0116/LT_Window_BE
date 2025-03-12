using System;
using Microsoft.EntityFrameworkCore;
using src.Models;

namespace src.Data;

public class ApplicationDbContext: DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
             : base(options)
         {
         }

         public DbSet<ItemGroup> ItemGroups { get; set; }
         public DbSet<Manufacturer> Manufacturers { get; set; }
         public DbSet<Item> Items { get; set; }
         public DbSet<Variant> Variants { get; set; }
         public DbSet<Inventory> Inventory { get; set; }
         public DbSet<Customer> Customers { get; set; }
         public DbSet<Invoice> Invoices { get; set; }
         public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
                  public DbSet<Color> Colors { get; set; }

          protected override void OnModelCreating(ModelBuilder modelBuilder)
         {
             base.OnModelCreating(modelBuilder);
         }

}
