using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InventoryManagement.Shared.DTO
{
    public class CreateUpdateCategoryDto
    {
        [MaxLength (100 , ErrorMessage = "Name must be less than 100 characters") ]
        [Required (ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!; 
        public int ? CategoryId { get; set; }   
    }
}
