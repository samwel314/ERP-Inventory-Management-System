using AutoMapper;
using AutoMapper.QueryableExtensions;
using InventoryManagement.Application.DTO;
using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace InventoryManagement.Application.Services
{
    public class ProductService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webenvironment;

        public ProductService(IUnitOfWork db, IMapper mapper, IWebHostEnvironment webenvironment)
        {
            _db = db;
            _mapper = mapper;
            _webenvironment = webenvironment;
        }
        // Create 
        public async Task<Result<ProductDTO>> CreateProductAsync(CreateProductDTO dto ,CancellationToken ct )
        {
            // check category 404 
            var categoryExist = await CategoryExist(dto.CategoryId, ct); 
            if (!categoryExist)
               return Result<ProductDTO>.Failure("This category Not Found ", ErrorType.NotFound);
            // 400
            var sameNameExist = await SameNameInCategoryExist(dto.CategoryId, dto.Name!, ct);  
            if (sameNameExist)
                return Result<ProductDTO>.Failure("You already have product with this name in the category ", ErrorType.Conflict);
             // 400
            var sameSDKExist = await SKDExist(dto.SKU!, ct);
            if (sameSDKExist)
                return Result<ProductDTO>.Failure("You already have product with this SKU  ", ErrorType.Conflict);

            var imageUrl = await  SaveImageAsync(dto.Image!);

            var Product = new Product(
                dto.Name!,
                dto.SKU!,
                dto.SellingPrice,
                dto.PurchasePrice,
                dto.MinimumStock,
                imageUrl,
                dto.CategoryId,
                dto.Description);
            await _db.Products.CreateAsync(Product); 
            await _db.SaveChangesAsync();   
            var productDTO = _mapper.Map<ProductDTO>(Product);      
            return Result<ProductDTO>.Success(productDTO); 
        }
        private async Task<bool> CategoryExist (int id , CancellationToken ct )
        {
            return await _db.Categories.GetAll().AnyAsync(c => c.Id == id , ct );
        }
        private async Task<bool> SameNameInCategoryExist(int categoryId,string productName ,  CancellationToken ct)
        {
            return await _db.Products.GetAll().
                AnyAsync
                (p=> p.CategoryId ==categoryId && 
                p.Name.ToLower() == productName.ToLower(), ct);
        }
        private async Task<bool> SKDExist (string Skd , CancellationToken ct )
        {
            return await _db.Products.GetAll().
                AnyAsync
                (p => p.SKU.ToLower() == Skd.ToLower(), ct);
        }
        private async Task <string> SaveImageAsync(IFormFile image )
        {
            var webPath = _webenvironment.WebRootPath;
            var imagesPath = Path.Combine(webPath, "Images");
            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);      
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(imagesPath, fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create); 
            await image.CopyToAsync(fileStream);
            return $"/Images/{fileName}";;
        }

        // get all 
        public async Task <Result<Pagination<ProductDTO>>> GetAllProductsAsync(int page , int pageSize , CancellationToken ct = default)
        {
            var count =
                 await _db.Products.GetAll().CountAsync(ct);
            var pagination = new Pagination<ProductDTO>(count, pageSize, page); 
           var products = 
                await _db.Products.GetAll().
                ProjectTo<ProductDTO>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);
            pagination.Items = products;    
            return Result<Pagination<ProductDTO>>.Success(pagination);  
        }
  
        public async Task<Result<ProductDTO>> GetProductAsync(Guid Id)
        {
            var  product =
           await _db.Products.GetAll().
            Where(p=>p.Id == Id).
            ProjectTo<ProductDTO>(_mapper.ConfigurationProvider).
            FirstOrDefaultAsync();
            if (product == null)
                return Result<ProductDTO>.Failure("This Product Not Found", ErrorType.NotFound);

            return Result<ProductDTO>.Success(product); 
        }
    }
}
