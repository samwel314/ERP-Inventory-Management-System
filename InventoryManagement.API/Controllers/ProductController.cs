using InventoryManagement.Application.DTOS;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Application.Services;
using InventoryManagement.Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace InventoryManagement.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
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
        public async Task<IActionResult> Create(CreateProductApiDto dto, CancellationToken ct = default)
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
        [MapToApiVersion("2.0")]
        [HttpGet]
        public async Task<IActionResult> GetProductsDetails(int page = 1, int pageSize = 5, CancellationToken ct = default)
        {
            var result = await _productService.GetAllProductsWithDetailsAsync(page, pageSize, ct);
            return Ok(result.Data);
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 5,  bool ?active = null , int ? categoryId = null, string ? searchTerm  = null,    CancellationToken ct = default)
        {
            var result = await _productService.GetAllProductsAsync(page, pageSize, active , categoryId ,searchTerm,  ct);
            return Ok(result.Data);
        }


        [HttpPut("{id}/info")]
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
        [HttpPut("{id}/pricing")]
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
        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateProductImage(Guid id, UpdateProductImageApiDTO dto, CancellationToken ct = default)
        {
            var result = await _productService.UpdateProductImageAsync(id, dto, ct);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return NoContent();
        }
        [HttpPut("{Id}/activate")]
        public async Task<IActionResult> ActiveProduct(Guid Id, CancellationToken ct = default)
        {
            var result = await _productService.ActiveProductAsync(Id, ct);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return NoContent(); ;
        }
        [HttpPut("{Id}/deactivate")]
        public async Task<IActionResult> DeActiveProduct(Guid Id, CancellationToken ct = default)
        {
            var result = await _productService.DeActiveProductAsync(Id, ct);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return NoContent(); ;
        }

    }
}