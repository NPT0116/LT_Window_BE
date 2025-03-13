using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models;

   public class Item
    {
        [Key]
        public Guid ItemID { get; set; }
        
        [ForeignKey("ItemGroup")]
        public Guid? ItemGroupID { get; set; }
        public ItemGroup? ItemGroup { get; set; }

        [Required]
        public string ItemName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? Picture { get; set; }
        
        public DateTime ReleaseDate { get; set; }

        [ForeignKey("Manufacturer")]
        public Guid ManufacturerID { get; set; }
        public Manufacturer Manufacturer { get; set; }

        public ICollection<Variant> Variants { get; set; } = new List<Variant>();
        public ICollection<Color> Colors { get; set; } = new List<Color>();
    }