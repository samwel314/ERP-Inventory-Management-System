using FluentValidation;
using InventoryManagement.Shared.DTO;

namespace InventoryManagement.Shared.Validations
{
    public class UpdateProductSKUDTOValidator : AbstractValidator<UpdateProductSKUDTO>
    {
        public UpdateProductSKUDTOValidator()
        {
            RuleFor(m => m.SKU).ValidSKU().When(o => o.SKU != null);
        }
    }
}
