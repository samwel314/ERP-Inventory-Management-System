namespace InventoryManagement.Application.DTO
{
    public class ProductListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal SellingPrice { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
