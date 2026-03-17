using Microsoft.AspNetCore.Http;

namespace InventoryManagement.Application.DTO
{
    public class UpdateProductImageDTO
    {
        public IFormFile ? Image { get; set; }
    }

}
