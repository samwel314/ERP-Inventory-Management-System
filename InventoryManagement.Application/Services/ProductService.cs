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
        public async Task<Result<ProductDTO>> CreateProductAsync(CreateProductDTO dto, CancellationToken ct)
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

            var imageUrl = await SaveImageAsync(dto.Image!);

            var Product = new Product(
                dto.Name!.ToLower().Trim(),
                dto.SKU!.ToLower()!.Trim(),
                dto.SellingPrice,
                dto.PurchasePrice,
                dto.MinimumStock,
                imageUrl,
                dto.CategoryId,
                dto.Description);
            await _db.Products.CreateAsync(Product);
            await _db.SaveChangesAsync(ct);
            var productDTO = _mapper.Map<ProductDTO>(Product);
            return Result<ProductDTO>.Success(productDTO);
        }
        private async Task<bool> CategoryExist(int id, CancellationToken ct)
        {
            return await _db.Categories.GetAll().AnyAsync(c => c.Id == id, ct);
        }
        private async Task<bool> SameNameInCategoryExist(int categoryId, string productName, CancellationToken ct)
        {
            return await _db.Products.GetAll().
                AnyAsync
                (p => p.CategoryId == categoryId &&
                p.Name.ToLower() == productName.ToLower(), ct);
        }
        private async Task<bool> SKDExist(string Skd, CancellationToken ct)
        {
            return await _db.Products.GetAll().
                AnyAsync
                (p => p.SKU.ToLower() == Skd.ToLower().Trim(), ct);
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

        // get all 
        public async Task<Result<Pagination<ProductDTO>>> GetAllProductsAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var count =
                 await _db.Products.GetAll().CountAsync(ct);
            var pagination = new Pagination<ProductDTO>(count, pageSize, page);
            var products =
                 await _db.Products.GetAll().Skip((pagination.pageNumber - 1  )* pagination.pageSize).Take(pagination.pageSize).
                 ProjectTo<ProductDTO>(_mapper.ConfigurationProvider)
                 .ToListAsync(ct);
            pagination.Items = products;
            return Result<Pagination<ProductDTO>>.Success(pagination);
        }

        public async Task<Result<ProductDTO>> GetProductAsync(Guid Id)
        {
            var product =
           await _db.Products.GetAll().
            Where(p => p.Id == Id).
            ProjectTo<ProductDTO>(_mapper.ConfigurationProvider).
            FirstOrDefaultAsync();
            if (product == null)
                return Result<ProductDTO>.Failure("This Product Not Found", ErrorType.NotFound);

            return Result<ProductDTO>.Success(product);
        }
        public async Task<Result<string>> UpdateProductBasicInfoAsync(Guid Id, UpdateProductBasicInfoDTO dto, CancellationToken ct = default)
        {
            // load product 
            var product = await _db.Products.GetByIdAsync(Id, ct, true);
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
                product.UpdateDescription(dto.Description);
            if (dto.MinimumStock != null)
                product.UpdateMinimumStock(dto.MinimumStock.Value);
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success();
        }
        public async Task<Result<string>> UpdateProductPricingAsync(Guid Id, UpdateProductPricingDTO dto, CancellationToken ct = default)
        {
            var product = await _db.Products.GetByIdAsync(Id, ct, true);
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
            var product = await _db.Products.GetByIdAsync(Id, ct, true);
            if (product == null)
                return Result<string>.Failure("This Product Not Found", ErrorType.NotFound);
            if (!product.SKU.Equals(dto.SKU!.ToLower().Trim() , StringComparison.OrdinalIgnoreCase))
            {
                var sameSDKExist = await SKDExist(dto.SKU!, ct);
                if (sameSDKExist)
                    return Result<string>.Failure("You already have product with this SKU  ", ErrorType.Conflict);
                product.UpdateSKU(dto.SKU!.ToLower().Trim());
                await _db.SaveChangesAsync(ct);
            }
            return Result<string>.Success();
        }

        public async Task <Result<string>> UpdateProductImageAsync (Guid Id, UpdateProductImageDTO dto, CancellationToken ct = default)
        {
            var product = await _db.Products.GetByIdAsync(Id, ct, true);
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
            path = path.Remove(0, 1); 
            var webPath = _webenvironment.WebRootPath;          
            var imagePath = Path.Combine(webPath, path);     
            if (!File.Exists(imagePath)) return;
            File.Delete(imagePath);        
        }

        public async Task<Result<string>> ActiveProductAsync(Guid Id, CancellationToken ct = default)
        {
            var product = await _db.Products.GetByIdAsync(Id, ct, true);
            if (product == null)
                return Result<string>.Failure("This product not Found", ErrorType.NotFound);
            product.Activate();
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success();
        }
        public async Task<Result<string>> DeActiveProductAsync(Guid Id, CancellationToken ct = default)
        {
            var product = await _db.Products.GetByIdAsync(Id, ct, true);
            if (product == null)
                return Result<string>.Failure("This product not Found", ErrorType.NotFound);
            product.Deactivate();
            await _db.SaveChangesAsync(ct);
            return Result<string>.Success();
        }

    }
}
