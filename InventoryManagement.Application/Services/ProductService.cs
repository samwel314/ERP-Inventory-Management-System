using AutoMapper;
using AutoMapper.QueryableExtensions;
using InventoryManagement.Application.DTO;
using InventoryManagement.Shared.DTO;
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
using System.Net.WebSockets;
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
        public async Task<Result<ProductDetailsDTO>> CreateProductAsync(CreateProductDTO dto, CancellationToken ct)
        {
            // check category 404 
            var categoryExist = await CategoryExist(dto.CategoryId, ct);
            if (!categoryExist)
                return Result<ProductDetailsDTO>.Failure("This category Not Found ", ErrorType.NotFound);
            // 400
            var sameNameExist = await SameNameInCategoryExist(dto.CategoryId, dto.Name!, ct);
            if (sameNameExist)
                return Result<ProductDetailsDTO>.Failure("You already have product with this name in the category ", ErrorType.Conflict);
            // 400
            var sameSDKExist = await SKUExist(dto.SKU!, ct);
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
        private async Task<bool> CategoryExist(int id, CancellationToken ct)
        {
            return await _db.Categories.GetAll().AnyAsync(c => c.Id == id, ct);
        }
        private async Task<bool> SameNameInCategoryExist(int categoryId, string productName, CancellationToken ct)
        {
            // sql server by default not case Sensitive
            // productName = productName.Trim().ToLower();    
            return await _db.Products.GetAll().
                AnyAsync
                (p => p.CategoryId == categoryId &&
                p.Name == productName, ct);
        }
        private async Task<bool> SKUExist(string Skd, CancellationToken ct)
        {
            return await _db.Products.GetAll().
                AnyAsync
                (p => p.SKU == Skd, ct);
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
        public async Task<Result<Pagination<ProductListDTO>>> GetAllProductsAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var count =
                 await _db.Products.GetAll().CountAsync(ct);
            var pagination = new Pagination<ProductListDTO>(count, pageSize, page);
            var products =
                 await _db.Products.GetAll().Skip((pagination.pageNumber - 1) * pagination.pageSize).Take(pagination.pageSize).
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
            // load product 
            var product = await GetProductAsync(Id, ct); 
            if (product == null)
                return Result<string>.Failure("This Product Not Found", ErrorType.NotFound);
            // Work with Category 
            if (dto.CategoryId != null)
            {
                var categoryExist = await CategoryExist(dto.CategoryId.Value, ct);
                if (!categoryExist)
                    return Result<string>.Failure("This Category Not Found", ErrorType.NotFound);
                product.ChangeCategory(dto.CategoryId.Value);
            }
            // WORK with name
            if (dto.Name != null) // my not pass a name 
            {
                if (!product.Name.Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase)) // new name not equal to old name
                {
                    var sameNameExist = await SameNameInCategoryExist(product.CategoryId, dto.Name!, ct);
                    if (sameNameExist)
                        return Result<string>.Failure("You already have product with this name in the category ", ErrorType.Conflict);
                    product.UpdateName(dto.Name.Trim().ToLower());
                }
            }
            if (dto.Description != null && product.Description != dto.Description)
                product.UpdateDescription(dto.Description.Trim());
            if (dto.MinimumStock != null)
                product.UpdateMinimumStock(dto.MinimumStock.Value);
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success();
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
                var sameSDKExist = await SKUExist(dto.SKU!, ct);
                if (sameSDKExist)
                    return Result<string>.Failure("You already have product with this SKU  ", ErrorType.Conflict);
                product.UpdateSKU(dto.SKU!.ToLower().Trim());
                await _db.SaveChangesAsync(ct);
            }
            return Result<string>.Success();
        }

        public async Task <Result<string>> UpdateProductImageAsync (Guid Id, UpdateProductImageDTO dto, CancellationToken ct = default)
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
