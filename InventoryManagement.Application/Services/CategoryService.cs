using AutoMapper;
using InventoryManagement.Application.DTO;
using InventoryManagement.Application.Mapping;
using InventoryManagement.Application.Persistence;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Application.Services
{
    public class CategoryService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<CategoryDTO>> CreateCategoryAsync(CreateCategoryDto model)
        {
            var exist = await IsNameExist(model.Name);
            if (exist)
                return Result<CategoryDTO>.Failure("Category name already exists.");
            var category = new Category(model.Name);
            await _db.Categories.CreateAsync(category);
            await _db.SaveChangesAsync();
            return Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(category));
        }

        private async Task<bool> IsNameExist(string name)
        {
            return await _db.Categories.GetAll().AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
       
        public async Task<Result<CategoryDTO>> GetByIdAsync(int id)
        {
            var category = await _db.Categories.GetByIdAsync(id);
            if (category == null)
                return Result<CategoryDTO>.Failure("Category not found.");
            return  Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(category)); 
        }
    }
}
