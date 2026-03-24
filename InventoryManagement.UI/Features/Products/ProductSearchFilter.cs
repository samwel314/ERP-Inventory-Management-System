namespace InventoryManagement.UI.Features.Products
{
    public class ProductSearchFilter
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int Next { get; set; }
        public int Previous { get; set; }
        public int TotalPages { get; set; }
    }
}
