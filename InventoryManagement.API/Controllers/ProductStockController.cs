using InventoryManagement.Shared.DTO;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Route("api/productStock")]
    [ApiController]
    public class ProductStockController : ControllerBase
    {
        private readonly ProductStockService _service;
        public ProductStockController(ProductStockService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int? warehouseId = null , string ? searchTerm = null,  CancellationToken ct = default)
        {
            var result = await _service.GetAllAsync(warehouseId , searchTerm ,  ct);
            return Ok(result.Data);
        }   
    }
}