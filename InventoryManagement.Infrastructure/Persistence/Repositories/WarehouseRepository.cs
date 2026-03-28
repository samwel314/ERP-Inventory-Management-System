using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagement.Infrastructure.Persistence.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ApplicationDbContext _db;
        public WarehouseRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task CreateAsync(Warehouse warehouse, CancellationToken ct = default)
        {
            await _db.Warehouses.AddAsync(warehouse, ct);
        }
        public void Delete(Warehouse warehouse)
        {
            _db.Warehouses.Remove(warehouse);
        }
        public IQueryable<Warehouse> GetAll()
        {
            return _db.Warehouses.AsNoTracking();
        }
        public async Task<Warehouse?> GetByIdAsync(int id, CancellationToken ct = default, bool track = false)
        {
            return track ? await _db.Warehouses.FirstOrDefaultAsync(w => w.Id == id, ct) :
                await _db.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, ct);
        }
    }
}
