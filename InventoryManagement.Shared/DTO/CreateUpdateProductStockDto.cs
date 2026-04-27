using InventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InventoryManagement.Shared.DTO
{
    public class CreateUpdateProductStockDto
    {
        [Required(ErrorMessage = "From Warehouse is required")]
        public int ? FromWarehouseId { get; set; }
        [Required(ErrorMessage = "Select a product")]
        public Guid ? ProductId { get;  set; }
        public int Quantity { get;  set; } // return , sell , push 
        /// ده لو في Row تاني هيتأثر 
        [Required(ErrorMessage = "Action type is required")]
        [EnumDataType(typeof(MovementType), ErrorMessage = "Invalid Movement Type")]
        public MovementType Type { get; set; }
        [TransferOperation]
        public int? ToWarehouseId { get; set; }
    }
    public class TransferOperation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var actionTypeProperty = instance.GetType().GetProperty("Type");
            if (actionTypeProperty != null)
            {
                var actionTypeValue = actionTypeProperty.GetValue(instance);

                if (actionTypeValue?.ToString() == "Transfer" || (int?)actionTypeValue == 2)
                {
                    if (value == null || (value is int intValue && intValue <= 0))
                    {
                        return new ValidationResult(ErrorMessage ?? "To Warehouse is required for transfers");
                    }
                }
            }
            return ValidationResult.Success;
        }

    }

}
