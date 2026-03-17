using FluentValidation;
using InventoryManagement.Application.DTO;

namespace InventoryManagement.Application.Validations
{
    public class UpdateProductSKUDTOValidator : AbstractValidator<UpdateProductSKUDTO>
    {
        public UpdateProductSKUDTOValidator()
        {
            RuleFor(m => m.SKU).ValidSKU();
        }
    }
}
