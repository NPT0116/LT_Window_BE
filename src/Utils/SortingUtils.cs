using System;
using System.Linq;
using src.Models;

namespace src.Utils
{
    public static class SortingUtils
    {
        /// <summary>
        /// Sắp xếp query theo Storage, chuyển đổi chuỗi sang giá trị số theo đơn vị GB.
        /// Lưu ý: Sử dụng AsEnumerable() để thực hiện sắp xếp trên bộ nhớ.
        /// </summary>
        public static IQueryable<Variant> SortByStorage(IQueryable<Variant> query, string sortDirection)
        {
            if (sortDirection.ToLower() == "desc")
            {
                return query.AsEnumerable()
                    .OrderByDescending(v => ConvertStorageToGB(v.Storage))
                    .AsQueryable();
            }
            else
            {
                return query.AsEnumerable()
                    .OrderBy(v => ConvertStorageToGB(v.Storage))
                    .AsQueryable();
            }
        }

        /// <summary>
        /// Chuyển đổi chuỗi lưu trữ (ví dụ "128GB", "1TB") sang giá trị số theo đơn vị GB.
        /// Nếu không parse được, trả về 0.
        /// </summary>
        private static double ConvertStorageToGB(string storage)
        {
            if (string.IsNullOrWhiteSpace(storage))
                return 0;
            
            storage = storage.Trim().ToUpper();

            if (storage.EndsWith("GB"))
            {
                var numberPart = storage.Substring(0, storage.Length - 2).Trim();
                if (double.TryParse(numberPart, out double value))
                {
                    return value;
                }
            }
            else if (storage.EndsWith("TB"))
            {
                var numberPart = storage.Substring(0, storage.Length - 2).Trim();
                if (double.TryParse(numberPart, out double value))
                {
                    return value * 1024; // Giả sử 1TB = 1024GB
                }
            }
            return 0;
        }
    }
}
