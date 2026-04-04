using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InventoryManagement.Shared.DTO
{
    public class CreateProductBaseDTO
    {
        public string ? Name { get;  set; } = null!;
        public string? Description { get;  set; }
        public string ? SKU { get;  set; } = null!;
        public int MinimumStock { get;  set; }
        public decimal SellingPrice { get;  set; }
        public decimal PurchasePrice { get;  set; }
        public int CategoryId { get;  set; }
    }
}
