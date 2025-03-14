using System;

namespace src.Exceptions.Invoice;

public class InvoiceNotFound: BaseException
{
    public InvoiceNotFound(Guid invoiceId) : base($"Invoice {invoiceId} not found", System.Net.HttpStatusCode.NotFound)
    {
    }
}
