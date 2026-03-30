using FluentValidation;
using InventoryManagement.Shared.DTO;

namespace InventoryManagement.Shared.Validations
{
    public class UpdateProductImageDTOValidator : AbstractValidator<UpdateProductImageDTO>
    {
        public UpdateProductImageDTOValidator()
        {
            RuleFor(m => m.Image).ValidImage().When(m=>m.Image != null);
        }
    }
}
