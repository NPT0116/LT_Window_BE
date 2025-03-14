using System;

namespace src.Exceptions.Customer;

public class CustomerPhoneExists: BaseException
{
    public CustomerPhoneExists(string phone) : base($"Customer with phone {phone} already exists.")
    {
    }
}   
