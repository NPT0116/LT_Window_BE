using System;

namespace src.Exceptions.Invoice;

public class TotalAmountNotMatchSumInvoiceDetail: BaseException 
{
    public TotalAmountNotMatchSumInvoiceDetail() : base("Total amount not match sum of invoice detail", System.Net.HttpStatusCode.BadRequest)
    {
    }
}
