using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Persistence.Repositories
{
    public interface IProductRepository
    {
        Task CreateAsync(Product Product, CancellationToken ct = default);
        void Delete(Product Product);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default, bool track = false);
        IQueryable<Product> GetAll();
    }

}
