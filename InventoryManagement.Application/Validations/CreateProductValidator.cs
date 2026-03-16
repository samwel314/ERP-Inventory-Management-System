using FluentValidation;
using InventoryManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace InventoryManagement.Application.Validations
{
    public class CreateProductValidator : AbstractValidator<CreateProductDTO>
    {
        static readonly List<string> Allows = new List<string>()
        {
            ".jpg" ,
            ".png" ,
        };
        public CreateProductValidator()
        {
            // اي LOGIC هيكون هنا هنا 
            RuleFor(m => m.Name)
                .NotEmpty().WithMessage("Product name is required ")
                .Length(2 , 100).WithMessage("Product name must be between 2 and 100 char");
            RuleFor(m => m.Description).MaximumLength(1000).WithMessage("Product Description must be less than 1000 char"); 
            RuleFor(m => m.SKU).NotEmpty().Length(4 , 100).WithMessage("Product SKU must be between 4 and 4 char");
            RuleFor(m => m.MinimumStock).GreaterThanOrEqualTo(0).WithMessage("Product MinimumStock should be great then or equal zero  ");
            RuleFor(m => m.SellingPrice).GreaterThanOrEqualTo(0.01m).WithMessage("Product SellingPrice should be at least 0.01 ");
            RuleFor(m => m.PurchasePrice).GreaterThanOrEqualTo(0.01m).WithMessage("Product PurchasePrice should be at least 0.01  ");
            RuleFor(m => m.CategoryId).GreaterThan(0).WithMessage("Invalid category id ");
            RuleFor(m => m.Image).Cascade(CascadeMode.Stop).NotNull().Must(i => Allows.Any(a => a == Path.GetExtension(i.FileName).ToLower())).WithMessage("allows types .jpg , .png"); 
        }
    }
}
