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
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost] 
        public async Task<IActionResult> Create(CreateProductDTO dto , CancellationToken ct = default )
        {

            var result = await _productService.CreateProductAsync(dto, ct);
            if (!result.IsSuccess)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.NotFound: return NotFound(new { message = result.ErrorMessage });
                    case ErrorType.Conflict: return Conflict(new { message = result.ErrorMessage });
                    default:
                        return BadRequest(result.ErrorMessage);
                }
            }
            return CreatedAtRoute(routeName: "GetProductById", routeValues: new
            {
                Id = result.Data!.Id
            }, result.Data);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public IActionResult GetById (int id )
        {
            return Ok(new { id });  
        }
    }
}