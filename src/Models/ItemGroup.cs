using System;
using System.ComponentModel.DataAnnotations;

namespace src.Models;

    public class ItemGroup
    {
        [Key]
        public Guid ItemGroupID { get; set; }
        
        [Required]
        public string ItemGroupName { get; set; } = string.Empty;
        
        
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
