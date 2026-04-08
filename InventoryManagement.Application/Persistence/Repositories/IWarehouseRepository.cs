using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Persistence.Repositories
{
    public interface IWarehouseRepository
    {
        Task CreateAsync(Warehouse warehouse, CancellationToken ct = default);
        void Delete(Warehouse warehouse);
        Task<Warehouse?> GetByIdAsync(int id, CancellationToken ct = default, bool track = false);
        IQueryable<Warehouse> GetAll();
        Task<bool> IsNameExist(string name, string city, int id = 0, CancellationToken ct = default);
    }

}
