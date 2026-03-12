using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Persistence.Repositories
{
    public interface ICategoryRepository
    {
        Task CreateAsync(Category category);
        void  Update (Category category);    
        void Delete (Category category);     
        Task <Category ? > GetByIdAsync (int id , bool track = false);    
        IQueryable<Category> GetAll ();    
    }
}
