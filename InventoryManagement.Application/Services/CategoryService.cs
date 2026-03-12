using AutoMapper;
using InventoryManagement.Application.DTO;
using InventoryManagement.Application.Mapping;
using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.ResultHelpers;
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

        public async Task<Result<CategoryDTO>> CreateCategoryAsync(CreateUpdateCategoryDto model)
        {
            var exist = await IsNameExist(model.Name);
            if (exist)
                return Result<CategoryDTO>.Failure("Category name already exists." , ErrorType.Conflict);
            var category = new Category(model.Name.ToLower());
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
                return Result<CategoryDTO>.Failure("Category not found." , ErrorType.NotFound);
            return  Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(category)); 
        }
        public async Task<Result<Pagination<CategoryDTO>>> GetCategoriesAsync(int  page  , int  pageSize)
        {
            var count  = await  _db.Categories.GetAll().CountAsync();
            Pagination<CategoryDTO> pagination = new Pagination<CategoryDTO>(count, pageSize,page);
            var Categories  = await _db.Categories.GetAll().Skip(pagination.pageSize * (pagination.pageNumber - 1)).Take(pageSize).ToListAsync();
            pagination.Items = _mapper.Map<IEnumerable<CategoryDTO>>(Categories); 
            return Result <Pagination<CategoryDTO>>.Success(pagination);
        }
        public async Task <Result <string>> UpdateCategoryAsync (int id ,  CreateUpdateCategoryDto model )
        {
            var category = await _db.Categories.GetByIdAsync(id, true);
            if (category == null)
                return Result<string>.Failure("This Category not Found" , ErrorType.NotFound);
            if (!category.Name.Equals(model.Name , StringComparison.OrdinalIgnoreCase))  
            {
                var exist = await IsNameExist(model.Name);
                if (exist)
                    return Result<string>.Failure("Category name already exists." , ErrorType.Conflict);
                category.UpdateName(model.Name); 
                await _db.SaveChangesAsync();     
            }
            return Result<string>.Success($"Category Updated Successfully"); 
        }


    }
}


