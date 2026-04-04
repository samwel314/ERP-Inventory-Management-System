using FluentValidation;
using InventoryManagement.Shared.DTO;
using InventoryManagement.Shared.Validations;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.UI.DTOS
{
    public class CreateProductUiDto : CreateProductBaseDTO
    {
        public IBrowserFile ? Image { get; set; } = null;
    }
    public class CreateProductUiValidator : AbstractValidator<CreateProductUiDto>
    {
        public CreateProductUiValidator()
        {
            Include(new CreateProductValidator());
            RuleFor(m => m.Image).NotNull().
                Must(i => i != null && 
                ProductValidationRules
                .Allows.Contains(Path.GetExtension(i.Name).ToLower())).WithMessage("allows types .jpg , .png");
        }
    }
}
