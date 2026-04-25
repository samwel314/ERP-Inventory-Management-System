namespace InventoryManagement.Domain.Entities
{
    public class StockMovement
    {
        public int Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int FromWarehouseId { get; private set; }
        public int? ToWarehouseId { get; private set; } 
        public int Quantity { get; private set; }
        public MovementType Type { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        //Nav 
        public Product Product { get; private set; } = null!;
        public Warehouse FromWarehouse { get; private set; } = null!;
        public Warehouse? ToWarehouse { get; private set; }

        private StockMovement() { }

        public StockMovement(Guid productId, int fromWarehouseId, int quantity, MovementType type, int? toWarehouseId = null)
        {
            ProductId = productId;
            FromWarehouseId = fromWarehouseId;
            Quantity = quantity;
            Type = type;
            ToWarehouseId = toWarehouseId;

            // Validation بسيط
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
            if (type == MovementType.Transfer && toWarehouseId == null)
                throw new ArgumentException("ToWarehouseId is required for transfers");
        }
    }
}
