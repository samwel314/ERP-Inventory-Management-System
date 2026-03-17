using FluentValidation;
using InventoryManagement.Application.DTO;

namespace InventoryManagement.Application.Validations
{
    public class UpdateProductImageDTOValidator : AbstractValidator<UpdateProductImageDTO>
    {
        public UpdateProductImageDTOValidator()
        {
            RuleFor(m => m.Image).ValidImage();
        }
    }
}
