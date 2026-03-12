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
        public async Task CreateAsync(Category category)
        {
            await _db.Categories.AddAsync(category);       
        }
        public void Delete(Category category)
        {
            _db.Categories.Remove(category);    
        }
        public IQueryable<Category> GetAll()
        {
            return _db.Categories.AsNoTracking();   
        }
        public async Task<Category?> GetByIdAsync(int id , bool track = false)
        {
            Category ? categoryFromDb; 
           if (track)
                categoryFromDb = await _db.Categories.FindAsync(id);    
           else
                categoryFromDb = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c=> c.Id == id); 
            return categoryFromDb ; 
        }
        public void Update(Category category)
        {
            _db.Categories.Update(category);        
        }
    }
}
