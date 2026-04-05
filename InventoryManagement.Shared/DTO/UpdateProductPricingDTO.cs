namespace InventoryManagement.Shared.DTO
{
    public class UpdateProductPricingDTO
    {
        public Guid Id { get; set; }
        public decimal ? SellingPrice { get; set; }
        public decimal ? PurchasePrice { get; set; }
    }

}
