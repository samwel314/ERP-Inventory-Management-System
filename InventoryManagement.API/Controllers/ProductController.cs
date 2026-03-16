using InventoryManagement.Application.DTO;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public ProductController()
        {
        }
        [HttpPost] 
        public async Task<IActionResult> Create(CreateProductDTO dto , CancellationToken ct = default )
        {

            return Ok(dto);     
        }
        
    }
}