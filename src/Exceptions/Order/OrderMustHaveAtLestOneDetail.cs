using System;

namespace src.Exceptions.Order;

public class OrderMustHaveAtLestOneDetail: BaseException
{
    public OrderMustHaveAtLestOneDetail() : base("Đơn đặt hàng phải có ít nhất 1 chi tiết", System.Net.HttpStatusCode.BadRequest)
    {
    }
}
