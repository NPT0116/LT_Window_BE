using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models;

public class OrderDetail
{
    [Key]
    public Guid OrderDetailID { get; set; }

    [ForeignKey("Order")]
    public Guid OrderID { get; set; }
    public Order Order { get; set; }

    [ForeignKey("Variant")]
    public Guid VariantID { get; set; }
    public Variant Variant { get; set; }

    public int Quantity { get; set; }
    public float Price { get; set; }
}
