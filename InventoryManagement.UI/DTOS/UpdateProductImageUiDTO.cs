using FluentValidation;
using InventoryManagement.Shared.Validations;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace InventoryManagement.Shared.DTO
{
    public class UpdateProductImageUiDTO
    {
        public Guid Id { get; set; }        
        public IBrowserFile  ? Image { get; set; }
    }
    public class UpdateProductImageDTOUiValidator : AbstractValidator<UpdateProductImageUiDTO>
    {
        public UpdateProductImageDTOUiValidator()
        {
            RuleFor(m => m.Image).NotNull().Must(i => i != null && ProductValidationRules.Allows.Contains(Path.GetExtension(i.Name).ToLower())).WithMessage("allows types .jpg , .png");
        }
    }
}
