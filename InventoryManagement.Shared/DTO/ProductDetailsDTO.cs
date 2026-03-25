namespace InventoryManagement.Shared.DTO
{
    public class ProductDetailsDTO
    {
        public Guid Id { get; set; }    
        public string? Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? SKU { get; set; } = null!;
        public int MinimumStock { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public string CategoryName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!; 
        public bool IsActive { get; set; }  
        public DateTime  CreatedAt {  get; set; }
        public decimal  ProfitPerUnit { get; set; }
    }
}
