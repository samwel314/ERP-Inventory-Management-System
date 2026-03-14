using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Persistence.Repositories
{
    public interface ICategoryRepository
    {
        Task CreateAsync(Category category, CancellationToken ct = default);
        void Delete (Category category );     
        Task <Category ? > GetByIdAsync (int id , CancellationToken ct = default , bool track = false);    
        IQueryable<Category> GetAll ();    
    }
}
