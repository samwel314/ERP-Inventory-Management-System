using Microsoft.AspNetCore.Http;

namespace InventoryManagement.Shared.DTO
{
    public class UpdateProductImageDTO
    {
        public IFormFile ? Image { get; set; }
    }

}
