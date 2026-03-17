using FluentValidation;
using InventoryManagement.Application.DTO;

namespace InventoryManagement.Application.Validations
{
    public class UpdateProductPricingDTOValidator : AbstractValidator<UpdateProductPricingDTO>
    {
        public UpdateProductPricingDTOValidator()
        {
            RuleFor(m => m.SellingPrice).GreaterThanOrEqualTo(0.01m).When(o=>o.SellingPrice != null);
            RuleFor(m => m.PurchasePrice).GreaterThanOrEqualTo(0.01m).When(o => o.PurchasePrice != null);
        }
    }
}
