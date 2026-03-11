using InventoryManagement.Application.DTO;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Create(CreateCategoryDto dto )
        {
            var result = await _service.CreateCategoryAsync(dto); 
            if (!result.IsSuccess)
                return Conflict(new { message =  result.ErrorMessage });
            return CreatedAtRoute(routeName: "GetById", routeValues: new
            {
                Id = result.Data!.Id 
            } , result.Data); 
        }
        [HttpGet ("{Id}" , Name = "GetById")] 
        public IActionResult GetCategory (int Id)
        {
            var result =  _service.GetByIdAsync(Id).Result;
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }
    }
}
