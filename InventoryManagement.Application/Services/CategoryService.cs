using AutoMapper;
using AutoMapper.QueryableExtensions;
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
            var exist = await _db.Categories.IsNameExist(model.Name.Trim() , ct);
            if (exist)
                return Result<CategoryDTO>.Failure("Category name already exists." , ErrorType.Conflict);
            var category = new Category(model.Name.ToLower().Trim());
            await _db.Categories.CreateAsync(category , ct );
            await _db.SaveChangesAsync(ct);
            return Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(category));
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
            var Categories  = await _db.Categories.GetAll().Where(c=> c.IsActive)
                . ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider).ToListAsync(ct);
            return Result <IEnumerable<CategoryDTO>>.Success(Categories);
        }
        public async Task<Result<IEnumerable<CategoryListDTO>>>
            GetCategoriesAsync(string ? searchTerm = null , bool ? active = null , CancellationToken ct = default)
        {
            var CategoriesQuery =  _db.Categories.GetAll();
            if (active != null)
                CategoriesQuery = CategoriesQuery.Where(c => c.IsActive == active);
            if (searchTerm != null)
                CategoriesQuery = CategoriesQuery.Where(c => c.Name.Contains(searchTerm));
           var CategoriesDtoQuery =  CategoriesQuery
               .Select(c => new CategoryListDTO
               {
                   Id = c.Id,
                   Name = c.Name,
                   UpdatedAt = c.UpdatedAt,
                   CreatedAt = c.CreatedAt,
                   IsActive = c.IsActive,
                   ProductsCount = c.Products.Count(),
               });
            Console.WriteLine("----------*-*Show-*----------------");
            Console.WriteLine(CategoriesDtoQuery.ToQueryString());
            Console.WriteLine("----------*-*-*----------------");
            var Categories = await CategoriesDtoQuery.ToListAsync(ct);
            return Result<IEnumerable<CategoryListDTO>>.Success(Categories);
        }
        public async Task <Result <string>> UpdateCategoryAsync (int id ,  CreateUpdateCategoryDto model , CancellationToken ct = default)
        {
            var category = await _db.Categories.GetByIdAsync(id, ct , true);
            if (category == null)
                return Result<string>.Failure("This Category not Found" , ErrorType.NotFound);
            if (!category.Name.Equals(model.Name.Trim()))  //**---
            {
                var exist =  await _db.Categories.IsNameExist(model.Name.Trim() , ct );
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


