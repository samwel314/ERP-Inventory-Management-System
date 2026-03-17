using FluentValidation;
using InventoryManagement.Application.DTO;

namespace InventoryManagement.Application.Validations
{
    public class UpdateProductPricingDTOValidator : AbstractValidator<UpdateProductPricingDTO>
    {
        public UpdateProductPricingDTOValidator()
        {
            RuleFor(m => m.SellingPrice).ValidSellingPrice();
            RuleFor(m => m.PurchasePrice).ValidPurchasePrice();
        }
    }
}
