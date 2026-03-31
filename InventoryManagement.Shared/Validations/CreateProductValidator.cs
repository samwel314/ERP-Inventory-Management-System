using FluentValidation;
using InventoryManagement.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace InventoryManagement.Shared.Validations
{
    public class CreateProductValidator : AbstractValidator<CreateProductDTO>
    {
        public CreateProductValidator()
        {
            // اي LOGIC هيكون هنا هنا 
            RuleFor(m => m.Name).ValidName(); 
            RuleFor(m => m.Description).ValidDescription();
            RuleFor(m => m.SKU).ValidSKU();
            RuleFor(m => m.MinimumStock).ValidMinimumStock(); 
            RuleFor(m => m.SellingPrice).ValidSellingPrice();   
            RuleFor(m => m.PurchasePrice).ValidPurchasePrice();
            RuleFor(m => m.CategoryId).ValidCategoryId();   
           // RuleFor(m => m.Image).ValidImage(); 
        }
    }
}
