using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Persistence.Repositories
{
    public interface IStockMovementRepository
    {
        Task CreateAsync(StockMovement stockMovement, CancellationToken ct = default);
        IQueryable<StockMovement> GetAllProductMovement(Guid productId);
        IQueryable<StockMovement> GetAllWarehouseMovement(int warehouseId);
    }

}
