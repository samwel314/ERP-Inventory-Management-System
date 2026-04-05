using FluentValidation;
using Microsoft.AspNetCore.Http;
namespace InventoryManagement.Shared.Validations
{
    public static class ProductValidationRules
    {

        public static readonly List<string> Allows = new List<string>()
        {
            ".jpg" ,
            ".png" ,
        };
        public static IRuleBuilderOptions<T, string?> ValidName<T>(
            this IRuleBuilder<T, string?> rule)
        {
            return rule.NotEmpty().WithMessage("Product name is required ")
                .Length(2, 100).WithMessage("Product name must be between 2 and 100 char");
        }
        public static IRuleBuilderOptions<T , string? > ValidDescription<T>(this IRuleBuilder<T, string?> rule)
        {
            return rule.MaximumLength(1000).WithMessage("Product Description must be less than 1000 char");
        }
        public static IRuleBuilderOptions<T, string?> ValidSKU<T>(this IRuleBuilder<T, string?> rule)
        {
            return rule.NotEmpty().Length(4, 100).WithMessage("Product SKU must be between 4 and 100 char");
        }
        public static IRuleBuilderOptions<T, int> ValidMinimumStock<T>(this IRuleBuilder<T, int> rule)
        {
            return rule.GreaterThanOrEqualTo(0).WithMessage("Product MinimumStock should be great then or equal zero  ");
        }

        public static IRuleBuilderOptions<T, decimal> ValidSellingPrice<T>(this IRuleBuilder<T, decimal> rule)
        {
            return rule.GreaterThanOrEqualTo(0.01m).WithMessage("Product SellingPrice should be at least 0.01 ");
        }
        public static IRuleBuilderOptions<T, decimal> ValidPurchasePrice<T>(this IRuleBuilder<T, decimal> rule)
        {
            return rule.GreaterThanOrEqualTo(0.01m).WithMessage("Product PurchasePrice should be at least 0.01 ");
        }

        public static IRuleBuilderOptions<T, int> ValidCategoryId<T>(this IRuleBuilder<T, int> rule)
        {
            return rule.GreaterThan(0).WithMessage("Invalid category id ");
        }

    }
}
