using InventoryManagement.Application.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Application.Persistence
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IWarehouseRepository Warehouses  { get; }
        IProductStockRepository ProductStocks { get; }
        IStockMovementRepository StockMovements { get; }    
        Task SaveChangesAsync(CancellationToken ct );    
    }
}
