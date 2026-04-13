using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InventoryManagement.Domain.Entities
{
    public class ProductStock
    {
        public ProductStock(int warehouseId, Guid productId, int quantity)
        {
            Validate(warehouseId , productId  );
            ValidateQuantity(quantity); 
            WarehouseId = warehouseId;
            ProductId = productId;
            Quantity = quantity; 
        }
        private void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");
        }

        private bool CanWithdraw(int quantity) 
        {
            ValidateQuantity(quantity);
            return Quantity >= quantity;
        }
        public void IncreaseStock(int quantity) 
        {
            ValidateQuantity(quantity);
            Quantity += quantity; 
            LastUpdated = DateTime.UtcNow; 
        }
        public void DecreaseStock(int quantity) 
        {
            ValidateQuantity(quantity);
            if (!CanWithdraw(quantity))
                throw new InvalidOperationException("Not enough stock to withdraw the requested quantity");
            Quantity -= quantity; 
            LastUpdated = DateTime.UtcNow;
        }
        private void Validate(int warehouseId, Guid productId )
        {
            if (warehouseId <= 0)
                throw new ArgumentException("WarehouseId must be greater than 0");
            if (productId == Guid.Empty )
                throw new ArgumentException("ProductId must be a valid GUID");
        }
        public int WarehouseId { get; private set; }    
        public Guid ProductId { get; private set; } 
        public int Quantity { get; private set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow; // عشان اعرف تاريخ اخر مخزون 
        // nav 
        [ForeignKey("ProductId")]
        public Product Product { get; private set; } = null!;
        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; private set; } = null!;
    }
}
