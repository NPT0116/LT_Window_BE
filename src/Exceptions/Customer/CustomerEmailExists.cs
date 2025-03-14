using System;

namespace src.Exceptions.Customer;

public class CustomerEmailExists: BaseException
{
    public CustomerEmailExists(string email) : base($"Customer with email {email} already exists.", System.Net.HttpStatusCode.BadRequest)
    {
    }   
}
