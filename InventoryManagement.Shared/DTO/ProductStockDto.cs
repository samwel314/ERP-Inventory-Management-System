using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Shared.DTO
{
    public class ProductStockDto
    {
        public int WarehouseId { get;  set; }
        public Guid ProductId { get;  set; }
        public int Quantity { get;  set; }
        public DateTime LastUpdated { get;  set; }
        public string ProductName { get; set; } = null!; 
        public string WarehouseName { get; set; } = null!; 
        public string CategoryName { get; set; } = null!; 
        public string Sku   { get; set; } = null!;  
    }
}
