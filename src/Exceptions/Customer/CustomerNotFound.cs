using System;

namespace src.Exceptions.Customer;

public class CustomerNotFound: BaseException
{
    public CustomerNotFound(Guid customerId) : base($"Customer with ID {customerId} was not found.",System.Net.HttpStatusCode.NotFound)
    {
    }
}
