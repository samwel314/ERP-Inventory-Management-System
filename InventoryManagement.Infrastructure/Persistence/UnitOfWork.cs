using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Infrastructure.Persistence.Data;

namespace InventoryManagement.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db  ,
            ICategoryRepository  category , IProductRepository product , IWarehouseRepository warehouse ,
            IProductStockRepository productStock , IStockMovementRepository stockMovement )
        {
            _db = db;
            Categories = category;      
            Products = product;     
            Warehouses = warehouse; 
            ProductStocks = productStock;
            StockMovements = stockMovement;
        }
        public ICategoryRepository Categories { get;  }
        public IProductRepository Products { get; }
        public IWarehouseRepository Warehouses { get; }

        public IProductStockRepository ProductStocks { get; }

        public IStockMovementRepository StockMovements { get; }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
           await _db.SaveChangesAsync(ct);     
        }
    }
}
