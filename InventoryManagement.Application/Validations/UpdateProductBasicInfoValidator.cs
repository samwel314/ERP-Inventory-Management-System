using FluentValidation;
using InventoryManagement.Application.DTO;

namespace InventoryManagement.Application.Validations
{
    public class UpdateProductBasicInfoValidator : AbstractValidator <UpdateProductBasicInfoDTO>
    {
        public UpdateProductBasicInfoValidator()
        {
            RuleFor(m => m.Name).ValidName();
            RuleFor(m => m.Description).ValidDescription();
            RuleFor(m => m.CategoryId).GreaterThan(0).When(v=>v.CategoryId != null) ;
            RuleFor(m => m.MinimumStock).GreaterThanOrEqualTo(0).When(v => v.MinimumStock != null);
        }
    }
}
