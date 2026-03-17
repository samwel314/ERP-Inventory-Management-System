namespace InventoryManagement.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public string SKU { get; private set; } = null!;
        public int MinimumStock { get; private set; }
        public decimal SellingPrice { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public int CategoryId { get; private set; }
        public string ImageUrl { get; private set; } = null!;
        // nav
        public Category Category { get; private set; } = null!;
        // behavior 
        Product()
        {

        }
        public Product(string Name, string SKU, decimal SellingPrice, decimal PurchasePrice, int MinimumStock, string ImageUrl, int CategoryId, string? Description = null)
        {
            ValidateName(Name);
            ValidateSKU(SKU);
            ValidateSellingPrice(SellingPrice);
            ValidatePurchasePrice(PurchasePrice);
            ValidateMinimumStock(MinimumStock);
            ValidateImageUrl(ImageUrl);
            Id = Guid.NewGuid();
            this.Name = Name;
            this.SKU = SKU;
            this.SellingPrice = SellingPrice;
            this.PurchasePrice = PurchasePrice;
            this.MinimumStock = MinimumStock;
            this.ImageUrl = ImageUrl;
            this.CategoryId = CategoryId;
            this.Description = Description;
        }
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");
            if (name.Length > 100)
                throw new ArgumentException("Name must be less than 100 characters");
        }
        private void ValidateSKU(string SKU)
        {
            if (string.IsNullOrWhiteSpace(SKU))
                throw new ArgumentException("SKU is required");
        }
        private void ValidateMinimumStock(int minimumStock)
        {
            if (minimumStock < 0)
                throw new ArgumentException("minimumStock must be greater than zero or equal zero ");
        }
        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateSKU(string sku)
        {
            ValidateSKU(sku);
            SKU = sku;
            UpdatedAt = DateTime.UtcNow;
        }

        private void ValidateImageUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("ImageUrl is required");
        }
        private void ValidateSellingPrice(decimal sellingPrice)
        {
            if (sellingPrice <= 0)
                throw new ArgumentException("sellingPrice must be greater than zero ");
        }
        private void ValidatePurchasePrice(decimal purchasePrice)
        {
            if (purchasePrice <= 0)
                throw new ArgumentException("purchasePrice must be greater than zero ");
        }
        public void UpdateSellingPrice(decimal sellingPrice)
        {
            ValidateSellingPrice(sellingPrice);
            SellingPrice = sellingPrice;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdatePurchasePrice(decimal purchasePrice)
        {
            ValidatePurchasePrice(purchasePrice);
            PurchasePrice = purchasePrice;
            UpdatedAt = DateTime.UtcNow;
        }
        public decimal ProfitPerUnit()
        {
            return SellingPrice - PurchasePrice;
        }
        public void UpdateMinimumStock(int minimumStock)
        {
            ValidateMinimumStock(minimumStock);
            MinimumStock = minimumStock;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateImageUrl(string url)
        {
            ValidateImageUrl(url);
            ImageUrl = url;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateDescription(string description)
        {
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
        public void ChangeCategory(int categoryId)
        {
            CategoryId = categoryId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
