using InventoryManagement.Shared.DTO;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Route("api/Categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;
        public CategoryController(CategoryService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdateCategoryDto dto, CancellationToken ct = default)
        {
            var result = await _service.CreateCategoryAsync(dto, ct);
            if (!result.IsSuccess)
                return Conflict(new { message = result.ErrorMessage });
            return CreatedAtRoute(routeName: "GetById", routeValues: new
            {
                Id = result.Data!.Id
            }, result.Data);
        }
        [HttpGet("{Id}", Name = "GetById")]
        public async Task<IActionResult> GetCategory(int Id, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(Id, ct);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }
        [HttpGet("lookUp")]
        public async Task<IActionResult> GetCategoriesLookUp( CancellationToken ct = default)
        {
            var result = await _service.GetCategoriesLookUpAsync(ct);
            return Ok(result.Data);
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories(string? searchTerm = null, bool? active = null, CancellationToken ct = default)
        {
            var result = await _service.GetCategoriesAsync(searchTerm , active ,  ct);
            return Ok(result.Data);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateCategory(int Id, CreateUpdateCategoryDto Model, CancellationToken ct = default)
        {
            var result = await _service.UpdateCategoryAsync(Id, Model, ct);
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
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteCategory(int Id, CancellationToken ct = default)
        {
            var result = await _service.DeleteCategoryAsync(Id, ct);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return NoContent(); ;
        }
        [HttpPut("{Id}/activate")]
        public async Task<IActionResult> ActiveCategory(int Id, CancellationToken ct = default)
        {
            var result = await _service.ActiveCategoryAsync(Id, ct);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return NoContent(); ;
        }
        [HttpPut("{Id}/deactivate")]
        public async Task<IActionResult> DeActiveCategory(int Id, CancellationToken ct = default)
        {
            var result = await _service.DeActiveCategoryAsync(Id, ct);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return NoContent(); ;
        }

    }
}