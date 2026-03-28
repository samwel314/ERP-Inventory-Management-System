using InventoryManagement.Application.DTO;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Route("api/warehouses")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseService _service;
        public WarehouseController(WarehouseService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdateWarehouseDto dto, CancellationToken ct = default)
        {
            var result = await _service.CreateWarehouseAsync(dto, ct);
            if (!result.IsSuccess)
                return Conflict(new { message = result.ErrorMessage });
            return CreatedAtRoute(routeName: "GetById", routeValues: new
            {
                Id = result.Data!.Id
            }, result.Data);
        }
        [HttpGet("{Id}", Name = "GetWarehouseById")]
        public async Task<IActionResult> GetWarehouse(int Id, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(Id, ct);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }
        [HttpGet("lookUp")]
        public async Task<IActionResult> GetWarehouseLookUp( CancellationToken ct = default)
        {
            var result = await _service.GetWarehousesLookUpAsync(ct);
            return Ok(result.Data);
        }
        [HttpGet]
        public async Task<IActionResult> GetWarehouses(string? searchTerm = null, bool? active = null, CancellationToken ct = default)
        {
            var result = await _service.GetWarehousesAsync(searchTerm , active ,  ct);
            return Ok(result.Data);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateWarehouse(int Id, CreateUpdateWarehouseDto Model, CancellationToken ct = default)
        {
            var result = await _service.UpdateWarehouseAsync(Id, Model, ct);
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
        [HttpPut("{Id}/activate")]
        public async Task<IActionResult> ActiveWarehouse(int Id, CancellationToken ct = default)
        {
            var result = await _service.ActiveWarehouseAsync(Id, ct);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return NoContent(); ;
        }
        [HttpPut("{Id}/deactivate")]
        public async Task<IActionResult> DeActiveWarehouse(int Id, CancellationToken ct = default)
        {
            var result = await _service.DeActiveWarehouseAsync(Id, ct);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return NoContent(); ;
        }

    }
}