using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task CreateAsync(Product Product, CancellationToken ct =default)
        {
            await _db.Products.AddAsync(Product, ct );       
        }
        public void Delete(Product Product)
        {
            _db.Products.Remove(Product);    
        }
        public IQueryable<Product> GetAll()
        {
            return _db.Products.AsNoTracking();   
        }
        public async Task<Product?> GetByIdAsync(Guid id , CancellationToken ct = default , bool track = false)
        {
           if (track)
                return await _db.Products.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id , ct);
            else
                return await _db.Products.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(p=> p.Id == id , ct); 
        }
    }
}
