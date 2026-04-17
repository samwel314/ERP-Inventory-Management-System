using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task CreateAsync(Category category, CancellationToken ct =default)
        {
            await _db.Categories.AddAsync(category, ct );       
        }
        public void Delete(Category category)
        {
            _db.Categories.Remove(category);    
        }

        public Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _db.Categories.AnyAsync(c => c.Id == id , cancellationToken);
        }

        public IQueryable<Category> GetAll()
        {
            return _db.Categories.AsNoTracking();   
        }
        public async Task<Category?> GetByIdAsync(int id , CancellationToken ct = default , bool track = false)
        {
            Category ? categoryFromDb; 
           if (track)
                categoryFromDb = await _db.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id , ct);
            else
                categoryFromDb = await _db.Categories.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(c=> c.Id == id , ct); 
            return categoryFromDb ; 
        }

        public async Task<bool> IsNameExist(string name, CancellationToken ct = default)
        {
            return await _db.Categories.AnyAsync(c => c.Name== name, ct);
        }
    }
}
