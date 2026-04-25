using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagement.Infrastructure.Persistence.Repositories
{
    public class ProductStockRepository : IProductStockRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductStockRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task CreateAsync(ProductStock productStock, CancellationToken ct = default)
        {
            await _db.ProductStocks.AddAsync(productStock, ct);
        }
        public IQueryable<ProductStock> GetAll()
        {
            return _db.ProductStocks.AsNoTracking();
        }
        public async Task<ProductStock?> GetByIdAsync(int warehouseId, Guid productId, CancellationToken ct = default, bool track = false)
        {
            return track ? await _db.ProductStocks.FirstOrDefaultAsync(ps => ps.WarehouseId == warehouseId && ps.ProductId == productId, ct) :
                await _db.ProductStocks.AsNoTracking().FirstOrDefaultAsync(ps => ps.WarehouseId == warehouseId && ps.ProductId == productId , ct);
        }
        public IQueryable<ProductStock> GetStocksByProduct(Guid productId)
        {
            return _db.ProductStocks.AsNoTracking().Where(ps => ps.ProductId == productId);
        }
        public IQueryable<ProductStock> GetStocksByWarehouse(int warehouseId)
        {
            return _db.ProductStocks.AsNoTracking().Where(ps => ps.WarehouseId == warehouseId) ;
        }

        public async Task<bool> IsExist(int warehouseId, Guid productId, CancellationToken ct = default)
        {
            return await _db.ProductStocks.AnyAsync(ps => ps.WarehouseId == warehouseId && ps.ProductId == productId    , ct);
        }
    }
}
