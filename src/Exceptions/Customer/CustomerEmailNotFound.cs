using System;

namespace src.Exceptions.Customer;

public class CustomerEmailNotFound: BaseException
{
    public CustomerEmailNotFound(string email) : base($"Customer email {email} not found")
    {
    }
}
