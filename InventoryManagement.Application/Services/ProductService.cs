using AutoMapper;
using AutoMapper.QueryableExtensions;
using InventoryManagement.Application.DTOS;
using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using File = System.IO.File;

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
        public async Task<Result<ProductDetailsDTO>> CreateProductAsync(CreateProductApiDto dto, CancellationToken ct)
        {
            // check category 404 
            var categoryExist = await _db.Categories.ExistsByIdAsync(dto.CategoryId, ct);
            if (!categoryExist)
                return Result<ProductDetailsDTO>.Failure("This category Not Found ", ErrorType.NotFound);
            // 400
            var sameNameExist = await _db.Products.SameNameInCategoryExist(dto.CategoryId, dto.Name!, ct);
            if (sameNameExist)
                return Result<ProductDetailsDTO>.Failure("You already have product with this name in the category ", ErrorType.Conflict);
            // 400
            var sameSDKExist = await _db.Products.SKUExist(dto.SKU!, ct);
            if (sameSDKExist)
                return Result<ProductDetailsDTO>.Failure("You already have product with this SKU  ", ErrorType.Conflict);

            var imageUrl = await SaveImageAsync(dto.Image!);

            var Product = new Product(
                dto.Name!.Trim().ToLower(),
                dto.SKU!.Trim().ToLower(),
                dto.SellingPrice,
                dto.PurchasePrice,
                dto.MinimumStock,
                imageUrl,
                dto.CategoryId,
                dto.Description?.Trim());
            await _db.Products.CreateAsync(Product);
            await _db.SaveChangesAsync(ct);
            var productDTO = _mapper.Map<ProductDetailsDTO>(Product);
            return Result<ProductDetailsDTO>.Success(productDTO);
        }
 
   
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var webPath = _webenvironment.WebRootPath;
            var imagesPath = Path.Combine(webPath, "Images");
            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(imagesPath, fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(fileStream);
            return $"/Images/{fileName}"; ;
        }
        private async Task<Product?> GetProductAsync(Guid id, CancellationToken ct)
        {
            return await _db.Products.GetByIdAsync(id, ct, true);
        }        // get all 
        public async Task<Result<Pagination<ProductDetailsDTO>>> GetAllProductsWithDetailsAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var count =
                 await _db.Products.GetAll().CountAsync(ct);
            var pagination = new Pagination<ProductDetailsDTO>(count, pageSize, page);
            var products =
                 await _db.Products.GetAll().Skip((pagination.pageNumber - 1  )* pagination.pageSize).Take(pagination.pageSize).
                 ProjectTo<ProductDetailsDTO>(_mapper.ConfigurationProvider)
                 .ToListAsync(ct);
            pagination.Items = products;
            return Result<Pagination<ProductDetailsDTO>>.Success(pagination);
        }
        public async Task<Result<Pagination<ProductListDTO>>> GetAllProductsAsync(int page, int pageSize, bool? active = null, int? categoryId = null , string? searchTerm = null , CancellationToken ct = default)
        {
            var baseQuery = _db.Products.GetAll();

            if (categoryId != null)
                baseQuery = baseQuery.Where(p => p.CategoryId == categoryId);
   
            if (active != null)
                baseQuery = baseQuery.Where(p => p.IsActive == active);
            if (searchTerm != null)
                baseQuery = baseQuery.Where(p => p.Name.Contains(searchTerm)
                          || p.SKU.Contains(searchTerm));
            Console.WriteLine("----------*-*-*----------------");
            Console.WriteLine(baseQuery.ToQueryString());
            Console.WriteLine("----------*-*-*----------------");
            var count = await baseQuery.CountAsync(ct);
            var pagination = new Pagination<ProductListDTO>(count, pageSize, page);
            var products = await baseQuery.OrderBy(p=> p.Name).Skip((pagination.pageNumber - 1) * pagination.pageSize).Take(pagination.pageSize).
                 ProjectTo<ProductListDTO>(_mapper.ConfigurationProvider)
                 .ToListAsync(ct);
            pagination.Items = products;
            return Result<Pagination<ProductListDTO>>.Success(pagination);
        }

        public async Task<Result<ProductDetailsDTO>> GetProductAsync(Guid Id)
        {
            var product =
           await _db.Products.GetAll().
            Where(p => p.Id == Id).
            ProjectTo<ProductDetailsDTO>(_mapper.ConfigurationProvider).
            FirstOrDefaultAsync();
            if (product == null)
                return Result<ProductDetailsDTO>.Failure("This Product Not Found", ErrorType.NotFound);

            return Result<ProductDetailsDTO>.Success(product);
        }
        public async Task<Result<string>> UpdateProductBasicInfoAsync(Guid Id, UpdateProductBasicInfoDTO dto, CancellationToken ct = default)
        {
            var product = await GetProductAsync(Id, ct );
            if (product == null)
                return Result<string>.Failure("This Product Not Found", ErrorType.NotFound);

            var finalCategoryId = dto.CategoryId ?? product.CategoryId;
            var finalName = dto.Name?.Trim() ?? product.Name;

            bool isCategoryChanged = dto.CategoryId != null && dto.CategoryId != product.CategoryId;
            bool isNameChanged = dto.Name != null && !product.Name.Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase);

            if (isCategoryChanged || isNameChanged)
            {
                var sameNameExist = await _db.Products.SameNameInCategoryExist(finalCategoryId, finalName, ct);
                if (sameNameExist)
                    return Result<string>.Failure("A product with this name already exists in the selected category.", ErrorType.Conflict);
            }

            if (isCategoryChanged)
            {
                var categoryExist = await _db.Categories.ExistsByIdAsync(dto.CategoryId!.Value, ct);
                if (!categoryExist)
                    return Result<string>.Failure("Selected Category Not Found", ErrorType.NotFound);

                product.ChangeCategory(dto.CategoryId.Value);
            }

            if (isNameChanged)
            {
                product.UpdateName(dto.Name!.Trim().ToLower());
            }

            if (dto.Description != null && !string.Equals(product.Description, dto.Description.Trim()))
            {
                product.UpdateDescription(dto.Description.Trim());
            }

            if (dto.MinimumStock != null && product.MinimumStock != dto.MinimumStock.Value)
            {
                product.UpdateMinimumStock(dto.MinimumStock.Value);
            }

        
            await _db.SaveChangesAsync(ct);

            return Result<string>.Success("Product updated successfully");
        }
        public async Task<Result<string>> UpdateProductPricingAsync(Guid Id, UpdateProductPricingDTO dto, CancellationToken ct = default)
        {
            var product = await GetProductAsync(Id, ct);
            if (product == null)
                return Result<string>.Failure("This Product Not Found", ErrorType.NotFound);
            if (dto.PurchasePrice != null)
                product.UpdatePurchasePrice(dto.PurchasePrice.Value);
            if (dto.SellingPrice != null)
                product.UpdateSellingPrice(dto.SellingPrice.Value);
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success();
        }
        public async Task<Result<string>> UpdateProductSKUAsync(Guid Id, UpdateProductSKUDTO dto, CancellationToken ct = default)
        {
            var product = await GetProductAsync(Id, ct);
            if (product == null)
                return Result<string>.Failure("This Product Not Found", ErrorType.NotFound);
            if (!product.SKU.Equals(dto.SKU!.ToLower().Trim() , StringComparison.OrdinalIgnoreCase))
            {
                var sameSDKExist = await _db.Products.SKUExist(dto.SKU!, ct);
                if (sameSDKExist)
                    return Result<string>.Failure("You already have product with this SKU  ", ErrorType.Conflict);
                product.UpdateSKU(dto.SKU!.ToLower().Trim());
                await _db.SaveChangesAsync(ct);
            }
            return Result<string>.Success();
        }

        public async Task <Result<string>> UpdateProductImageAsync (Guid Id, UpdateProductImageApiDTO dto, CancellationToken ct = default)
        {
            var product = await GetProductAsync(Id, ct);
            if (product == null)
                return Result<string>.Failure("This Product Not Found", ErrorType.NotFound);
            if (dto.Image != null)
            {
                var imagePath = await SaveImageAsync(dto.Image!);
                DeleteImage(product.ImageUrl);
                product.UpdateImageUrl(imagePath); 
                await _db.SaveChangesAsync(ct);       
            }
            return Result<string>.Success();
        }
        private void DeleteImage (string path )
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            var webPath = _webenvironment.WebRootPath;          
            // نتاكد ان مفيش اي بداية /// خالص 
            var imagePath = Path.Combine(webPath, path.TrimStart('/'));     
            if (!File.Exists(imagePath)) return;
            File.Delete(imagePath);        
        }

        public async Task<Result<string>> ActiveProductAsync(Guid Id, CancellationToken ct = default)
        {
            var product = await GetProductAsync(Id, ct);
            if (product == null)
                return Result<string>.Failure("This product not Found", ErrorType.NotFound);
            product.Activate();
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success();
        }
        public async Task<Result<string>> DeActiveProductAsync(Guid Id, CancellationToken ct = default)
        {
            var product = await GetProductAsync(Id, ct);
            if (product == null)
                return Result<string>.Failure("This product not Found", ErrorType.NotFound);
            product.Deactivate();
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success();
        }

    }
}
