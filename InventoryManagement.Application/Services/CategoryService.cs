using AutoMapper;
using InventoryManagement.Application.DTO;
using InventoryManagement.Application.Mapping;
using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.DTO;
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

        public async Task<Result<CategoryDTO>> CreateCategoryAsync(CreateUpdateCategoryDto model, CancellationToken ct = default)
        {
            var exist = await IsNameExist(model.Name , ct);
            if (exist)
                return Result<CategoryDTO>.Failure("Category name already exists." , ErrorType.Conflict);
            var category = new Category(model.Name.ToLower().Trim());
            await _db.Categories.CreateAsync(category , ct );
            await _db.SaveChangesAsync(ct);
            return Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(category));
        }

        private async Task<bool> IsNameExist(string name, CancellationToken ct = default)
        {
            return await _db.Categories.GetAll().AnyAsync(c => c.Name.ToLower() == name.ToLower() , ct);
        }
        public async Task<Result<CategoryDTO>> GetByIdAsync(int id , CancellationToken ct)
        {
            var category = await _db.Categories.GetByIdAsync(id , ct);
            if (category == null)
                return Result<CategoryDTO>.Failure("Category not found." , ErrorType.NotFound);
            return  Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(category)); 
        }
        public async Task<Result<IEnumerable<CategoryDTO>>> GetCategoriesLookUpAsync(CancellationToken ct = default)
        {
            var Categories  = await _db.Categories.GetAll().ToListAsync(ct);
            var dtos  = _mapper.Map<IEnumerable<CategoryDTO>>(Categories); 
            return Result <IEnumerable< CategoryDTO >>.Success(dtos);
        }
        public async Task <Result <string>> UpdateCategoryAsync (int id ,  CreateUpdateCategoryDto model , CancellationToken ct = default)
        {
            var category = await _db.Categories.GetByIdAsync(id, ct , true);
            if (category == null)
                return Result<string>.Failure("This Category not Found" , ErrorType.NotFound);
            if (!category.Name.Equals(model.Name , StringComparison.OrdinalIgnoreCase))  
            {
                var exist = await IsNameExist(model.Name , ct );
                if (exist)
                    return Result<string>.Failure("Category name already exists." , ErrorType.Conflict);
                category.UpdateName(model.Name); 
                await _db.SaveChangesAsync(ct);     
            }
            return Result<string>.Success($"Category Updated Successfully"); 
        }
        public async Task<Result<string>> DeleteCategoryAsync(int Id , CancellationToken ct = default )
        {
            //check if it have products 
           var category = await _db.Categories.GetByIdAsync(Id  , ct );
            if (category == null)
                return Result<string>.Failure("This Category not Found", ErrorType.NotFound);
            _db.Categories.Delete(category);
           await _db.SaveChangesAsync(ct); 
            return Result<string>.Success("Category Deleted Successfully");
        }
        public async Task<Result<string>> ActiveCategoryAsync(int Id , CancellationToken ct = default )
        {
            var category = await _db.Categories.GetByIdAsync(Id , ct, true);
            if (category == null)
                return Result<string>.Failure("This Category not Found", ErrorType.NotFound);
            category.Activate();    
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success("Category Activated Successfully");
        }
        public async Task<Result<string>> DeActiveCategoryAsync(int Id , CancellationToken ct = default)
        {
            var category = await _db.Categories.GetByIdAsync(Id, ct, true);
            if (category == null)
                return Result<string>.Failure("This Category not Found", ErrorType.NotFound);
            category.Deactivate();
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success("Category DeActivated Successfully");
        }
    }
}


