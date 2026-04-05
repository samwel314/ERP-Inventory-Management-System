using FluentValidation;
using InventoryManagement.Shared.DTO;
using InventoryManagement.Shared.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Application.DTOS
{
    public class CreateProductApiDto : CreateProductBaseDTO
    {
        public IFormFile? Image { get; set; }  = null;  
    }
    public class CreateProductApiValidator : AbstractValidator<CreateProductApiDto>
    {

        public CreateProductApiValidator()
        {
            Include(new CreateProductValidator());
            RuleFor(m => m.Image).NotNull().Must(i => i != null && ProductValidationRules.Allows.Contains(Path.GetExtension(i.FileName).ToLower())).WithMessage("allows types .jpg , .png");
        }
    }
}

