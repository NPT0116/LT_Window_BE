using System;

namespace src.Query;

public class InvoiceQueryParameter
{
    public InvoiceDatetimeQueryParameter? invoiceDatetimeQueryParameter { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class InvoiceDatetimeQueryParameter 
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string sortDirection { get; set; } = "desc";
}
