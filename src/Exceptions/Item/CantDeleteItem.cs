using System;

namespace src.Exceptions.Item;

public class CantDeleteItem: BaseException
{
    public CantDeleteItem(Guid itemId) : base($"Không thể xóa Item có id: {itemId} vì có Variant đã được sử dụng trong hóa đơn hoặc giao dịch tồn kho.")
    {
    }
}
