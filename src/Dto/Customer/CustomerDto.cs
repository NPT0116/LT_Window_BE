using System;

namespace src.Dto.Customer;

public class CustomerDto
{
    public Guid CustomerID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}
