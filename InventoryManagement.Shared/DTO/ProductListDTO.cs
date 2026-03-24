namespace InventoryManagement.Shared.DTO
{
    public class ProductListDTO
    {
        public Guid Id { get; set; }
        public string SKU { get;  set; } = null!;
        public string ImageUrl { get;  set; } = null!;
        public string Name { get; set; } = null!;
        public decimal SellingPrice { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
