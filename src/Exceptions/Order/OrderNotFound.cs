using System;

namespace src.Exceptions.Order;

public class OrderNotFound: BaseException
{
    public OrderNotFound(Guid orderId) : base($"Không tìm thấy đơn hàng với ID: {orderId}", System.Net.HttpStatusCode.NotFound)
    {
    }
}
