using AutoMapper;
using AutoMapper.QueryableExtensions;
using InventoryManagement.Application.DTO;
using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services
{
    public class WarehouseService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;

        public WarehouseService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<WarehouseDTO>> CreateCategoryAsync(CreateUpdateWarehouseDto model, CancellationToken ct = default)
        {
            var exist = await IsNameExist(model.Name.Trim(), model.City.Trim(), ct);
            if (exist)
                return Result<WarehouseDTO>.Failure("Warehouse name already exists in this city", ErrorType.Conflict);
            var warehouse = new Warehouse(model.Name.Trim().ToLower() , model.City.Trim().ToLower());
            await _db.Warehouses.CreateAsync(warehouse, ct);
            await _db.SaveChangesAsync(ct);
            return Result<WarehouseDTO>.Success(_mapper.Map<WarehouseDTO>(warehouse));
        }

        private async Task<bool> IsNameExist(string name, string city , CancellationToken ct = default)
        {
            return await _db.Warehouses.GetAll().AnyAsync(w => w.Name == name && w.City == city , ct);
        }
        public async Task<Result<WarehouseDetailsDTO>> GetByIdAsync(int id, CancellationToken ct)
        {
            var warehouse = await _db.Warehouses.GetByIdAsync(id, ct);
            if (warehouse == null)
                return Result<WarehouseDetailsDTO>.Failure("Warehouse not found.", ErrorType.NotFound);
            return Result<WarehouseDetailsDTO>.Success(_mapper.Map<WarehouseDetailsDTO>(warehouse));
        }
        public async Task<Result<IEnumerable<WarehouseDTO>>> GetWarehousesLookUpAsync(CancellationToken ct = default)
        {
            var warehouses = 
                await _db.Warehouses.GetAll().Where(w => w.IsActive).
                ProjectTo<WarehouseDTO>(_mapper.ConfigurationProvider).ToListAsync(ct);
            return Result<IEnumerable<WarehouseDTO>>.Success(warehouses);
        }
        public async Task<Result<IEnumerable<WarehouseDetailsDTO>>>
            GetWarehousesAsync(string? searchTerm = null, bool? active = null, CancellationToken ct = default)
        {
            var WarehousesQuery = _db.Warehouses.GetAll();
            if (active != null)
                WarehousesQuery = WarehousesQuery.Where(w => w.IsActive == active);
            if (searchTerm != null)
                WarehousesQuery = WarehousesQuery.Where(w => w.Name.Contains(searchTerm) || w.City.Contains(searchTerm));
            var warehouses = await WarehousesQuery.ProjectTo< WarehouseDetailsDTO >(_mapper.ConfigurationProvider).ToListAsync(ct);
            return Result<IEnumerable<WarehouseDetailsDTO>>.Success(warehouses);
        }
        public async Task<Result<string>> UpdateWarehouseAsync(int id, CreateUpdateWarehouseDto model, CancellationToken ct = default)
        {
            var warehouse = await _db.Warehouses.GetByIdAsync(id, ct, true);
            if (warehouse == null)
                return Result<string>.Failure("This warehouse not Found", ErrorType.NotFound);

            bool isNameChanged = !warehouse.Name.Equals(model.Name.Trim(), StringComparison.OrdinalIgnoreCase);
            bool isCityChanged = !warehouse.City.Equals(model.City.Trim(), StringComparison.OrdinalIgnoreCase);

            if (isNameChanged || isCityChanged)
            {
                var exist = await IsNameExist(model.Name, model.City, ct); 

                if (exist)
                    return Result<string>.Failure("Warehouse name already exists in this city.", ErrorType.Conflict);

                if (isNameChanged) warehouse.UpdateName(model.Name.Trim().ToLower());
                if (isCityChanged) warehouse.UpdateCity(model.City.Trim().ToLower());
            }

            if (model.Address?.Trim() != warehouse.Address)
            {
                warehouse.UpdateAddress(model.Address?.Trim().ToLower());
            }

            await _db.SaveChangesAsync(ct);

            return Result<string>.Success("Warehouse Updated Successfully");
        }
        public async Task<Result<string>> ActiveWarehouseAsync(int Id, CancellationToken ct = default)
        {
            var warehouse = await _db.Warehouses.GetByIdAsync(Id, ct, true);
            if (warehouse == null)
                return Result<string>.Failure("This Warehouse not Found", ErrorType.NotFound);
            warehouse.Activate();
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success("Warehouse Activated Successfully");
        }
        public async Task<Result<string>> DeActiveWarehouseAsync(int Id, CancellationToken ct = default)
        {
            var warehouse = await _db.Warehouses.GetByIdAsync(Id, ct, true);
            if (warehouse == null)
                return Result<string>.Failure("This warehouse not Found", ErrorType.NotFound);
            warehouse.Deactivate();
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success("warehouse DeActivated Successfully");
        }
    }

}


