using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagement.Infrastructure.Persistence.Repositories
{
    public class StockMovementRepository : IStockMovementRepository
    {
        private readonly ApplicationDbContext _db;

        public StockMovementRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(StockMovement stockMovement, CancellationToken ct = default)
        {
            await _db.StockMovements.AddAsync(stockMovement, ct);
        }

        public IQueryable<StockMovement> GetAllProductMovement(Guid productId)
        {
            return _db.StockMovements
                .AsNoTracking()
                .Where(sm => sm.ProductId == productId)
                .OrderByDescending(sm => sm.CreatedAt); 
        }
        public IQueryable<StockMovement> GetAllWarehouseMovement(int warehouseId)
        {
            return _db.StockMovements
                .AsNoTracking()
                .Where(sm => sm.FromWarehouseId == warehouseId || sm.ToWarehouseId == warehouseId)
                .OrderByDescending(sm => sm.CreatedAt);
        }
    }
}
