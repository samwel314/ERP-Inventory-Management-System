using FluentValidation;
using InventoryManagement.Shared.Validations;
using Microsoft.AspNetCore.Http;

namespace InventoryManagement.Shared.DTO
{
    public class UpdateProductImageApiDTO
    {
        public IFormFile ? Image { get; set; }
    }
    public class UpdateProductImageDTOValidator : AbstractValidator<UpdateProductImageApiDTO>
    {
        public UpdateProductImageDTOValidator()
        {
            RuleFor(m => m.Image).NotNull().Must(i => i != null && ProductValidationRules.Allows.Contains(Path.GetExtension(i.FileName).ToLower())).WithMessage("allows types .jpg , .png");
        }
    }
}
