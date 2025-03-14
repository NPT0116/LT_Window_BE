using System;

namespace src.Exceptions.Customer;

public class CustomerPhoneNotFound: BaseException
{
    public CustomerPhoneNotFound(string phone) : base($"Customer phone {phone} not found")
    {
    }
}

