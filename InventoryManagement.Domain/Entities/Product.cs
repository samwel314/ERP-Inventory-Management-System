namespace InventoryManagement.Domain.Entities
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public string SKU { get; private set; } = null!;
        public int MinimumStock { get; private set; }       
        public decimal SellingPrice { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public int CategoryId { get; private set; }
        public Unit Unit { get; private set; }
        // nav
        public Category Category { get; private set; } = null!;
    }
}
