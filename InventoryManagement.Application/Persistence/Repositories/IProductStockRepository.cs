using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Persistence.Repositories
{
    public interface IProductStockRepository
    {
        Task CreateAsync(ProductStock productStock, CancellationToken ct = default);
        Task<ProductStock?> GetByIdAsync(int warehouseId, Guid productId, CancellationToken ct = default, bool track = false);
        IQueryable<ProductStock> GetAll();
        Task<bool> IsExist(int warehouseId, Guid productId, CancellationToken ct = default);
        IQueryable<ProductStock> GetStocksByWarehouse(int warehouseId);
        IQueryable<ProductStock> GetStocksByProduct(Guid productId);
    }
}
