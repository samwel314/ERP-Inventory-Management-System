using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Infrastructure.Persistence.Data;

namespace InventoryManagement.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db  , ICategoryRepository  category , IProductRepository product)
        {
            _db = db;
            Categories = category;      
            Products = product;     
        }

        public ICategoryRepository Categories { get;  }
        public IProductRepository Products { get; }

        public async Task SaveChangesAsync()
        {
           await _db.SaveChangesAsync();     
        }
    }
}
