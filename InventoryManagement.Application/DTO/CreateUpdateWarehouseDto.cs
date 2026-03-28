using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTO
{
    public class CreateUpdateWarehouseDto
    {
        [MaxLength(100, ErrorMessage = "Name must be less than 100 characters")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;
        [MaxLength(50, ErrorMessage = "City name  must be less than 50 characters")]
        [Required(ErrorMessage = "City name is required")]
        public string City { get; set; } = null!;
        [MaxLength(150, ErrorMessage = "City name  must be less than 150 characters")]
        public string ? Address { get; set; }   
    }
}
