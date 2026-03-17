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
        Task SaveChangesAsync(CancellationToken ct );    
    }
}
