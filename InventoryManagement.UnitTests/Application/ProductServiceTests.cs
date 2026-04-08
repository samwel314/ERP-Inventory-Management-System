using AutoMapper;
using AutoMapper.Configuration;
using InventoryManagement.Application.DTOS;
using InventoryManagement.Application.Mapping;
using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;

namespace InventoryManagement.UnitTests.Application
{
    public class ProductServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly ProductService _productService;
        private readonly Mock<ICategoryRepository> _mockCategoryRepo;
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IWebHostEnvironment> _mockWebenvironment;
        public ProductServiceTests()
        {

            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), new LoggerFactory())
                .CreateMapper();
            _mockWebenvironment = new Mock<IWebHostEnvironment>();
            _mockWebenvironment.Setup(m => m.WebRootPath).Returns("wwwroot");   
            _mockUow = new Mock<IUnitOfWork>();

            _productService = new ProductService(_mockUow.Object, _mapper, _mockWebenvironment.Object);
            _mockCategoryRepo = new Mock<ICategoryRepository>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockUow.Setup(r => r.Categories).Returns(_mockCategoryRepo.Object);
            _mockUow.Setup(r => r.Products).Returns(_mockProductRepo.Object);
        }
        [Fact]
        public async Task CreateProductAsync_ExistName_ReturnsFailureResult()
        {
            var productDto = new CreateProductApiDto
            {
                Name = "Existing Product",
                CategoryId = 1,
            };
            _mockCategoryRepo.Setup(r => r.ExistsByIdAsync(productDto.CategoryId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);
            _mockProductRepo.Setup
                (r => r.SameNameInCategoryExist
                 (productDto.CategoryId, productDto.Name, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _productService.CreateProductAsync(productDto, It.IsAny<CancellationToken>());
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.Conflict, result.ErrorType);
            _mockProductRepo.Verify(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task CreateProductAsync_ExistSKU_ReturnsFailureResult()
        {
            var productDto = new CreateProductApiDto
            {
                Name = "Existing Product",
                SKU = "EXISTSKU",
                CategoryId = 1,
            };
            _mockCategoryRepo.Setup(r => r.ExistsByIdAsync(productDto.CategoryId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);
            _mockProductRepo.Setup
                (r => r.SameNameInCategoryExist
                 (productDto.CategoryId, productDto.Name, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockProductRepo.Setup
             (r => r.SKUExist
              (productDto.SKU, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _productService.CreateProductAsync(productDto, It.IsAny<CancellationToken>());
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.Conflict, result.ErrorType);
            _mockProductRepo.Verify(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task CreateProductAsync_WhenUploadImage_SuccessWithImagePath()
        {
            Mock<IFormFile> mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("testimage.jpg");
            var productDto = new CreateProductApiDto
            {
                Name = "Existing Product",
                SKU = "EXISTSKU",
                SellingPrice = 100, 
                PurchasePrice = 100,    
                MinimumStock = 10,
                CategoryId = 1,
                Image = mockFile.Object 
            };
            _mockCategoryRepo.Setup(r => r.ExistsByIdAsync(productDto.CategoryId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);
            _mockProductRepo.Setup
                (r => r.SameNameInCategoryExist
                 (productDto.CategoryId, productDto.Name, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockProductRepo.Setup
               (r => r.SameNameInCategoryExist
             (productDto.CategoryId, productDto.Name, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockProductRepo.Setup
             (r => r.SKUExist
              (productDto.SKU, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _productService.CreateProductAsync(productDto, It.IsAny<CancellationToken>());
            
            Assert.Contains("/Images", result.Data!.ImageUrl);
            Assert.Equal(ErrorType.Success, result.ErrorType);
            _mockProductRepo.Verify(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task UpdateProductPricingAsync_IfProductNotFound_ReturnsFailureResult()
        {
            var productDto = new UpdateProductPricingDTO
            {
                Id = Guid.NewGuid(),
            };

            _mockProductRepo.Setup
                (r => r.GetByIdAsync(productDto.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null!);

            var result = await _productService.UpdateProductPricingAsync(productDto.Id, productDto, It.IsAny<CancellationToken>());
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            _mockUow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task UpdateProductBasicInfoAsync_WhenChangeDescriptionOnly_NotCallExistName()
        {
            var productDto = new UpdateProductBasicInfoDTO
            {
                ProductId = Guid.NewGuid(),
                Name = null,
                Description = "New Description",
                CategoryId = null,
            };
            var existingProduct = new Product("fakeProduct", "F-105-6", 10m, 15m, 1, "/images", 1);
            _mockProductRepo.Setup
                (r => r.GetByIdAsync(productDto.ProductId.Value, It.IsAny<CancellationToken>() , It.IsAny<bool>()) )
                     .ReturnsAsync(existingProduct);
            var result = await _productService.
                UpdateProductBasicInfoAsync( productDto.ProductId.Value, productDto, It.IsAny<CancellationToken>());
            _mockProductRepo.Verify(r => r.SameNameInCategoryExist(It.IsAny<int>(), It.IsAny<string>() , It.IsAny<CancellationToken>()), Times.Never);
            Assert.True(result.IsSuccess); 
            Assert.Equal(productDto.Description , existingProduct.Description);

            _mockUow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
