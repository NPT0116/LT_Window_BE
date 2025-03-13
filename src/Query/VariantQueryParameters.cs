using System;

namespace src.Query;

    public class VariantQueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? StorageFilter { get; set; }
        public string? ManufacturerFilter { get; set; }
        public string? SortBy { get; set; }
        public string SortDirection { get; set; } = "asc";
    }