using InventoryManagement.Application.DTO;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Application.Services;
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
        public async Task<IActionResult> Create( CreateProductDTO dto, CancellationToken ct = default)
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
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _productService.GetProductAsync(id);   
            if (!result.IsSuccess)
              return NotFound(new { message = result.ErrorMessage });       
            return Ok(result.Data);
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 5, CancellationToken ct = default)
        {
            var result = await _productService.GetAllProductsAsync(page, pageSize, ct);
            return Ok(result.Data);

        }


        [HttpPatch("{id}/info")]
        public async Task<IActionResult> UpdateProductBasicInfo(Guid id , UpdateProductBasicInfoDTO dto   ,CancellationToken ct = default)
        {
            var result = await _productService.UpdateProductBasicInfoAsync(id , dto ,ct );
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
            return NoContent();     
        }
        [HttpPatch("{id}/pricing")]
        public async Task<IActionResult> UpdateProductPricingInfo(Guid id, UpdateProductPricingDTO dto, CancellationToken ct = default)
        {
            var result = await _productService.UpdateProductPricingAsync(id, dto, ct);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage }); 
            return NoContent();
        }
        [HttpPatch("{id}/sku")]
        public async Task<IActionResult> UpdateProductSKUInfo(Guid id, UpdateProductSKUDTO dto, CancellationToken ct = default)
        {
            var result = await _productService.UpdateProductSKUAsync(id, dto, ct);
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
            return NoContent();
        }

    }
}