using AutoMapper;
using AutoMapper.Configuration;
using InventoryManagement.Application.Mapping;
using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.DTO;
using Microsoft.Extensions.Logging;
using Moq;

namespace InventoryManagement.UnitTests.Application
{
    public class CategoryServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly CategoryService _categoryService;
        private readonly Mock<ICategoryRepository> _mockRepo  ; 
        public CategoryServiceTests()
        {

            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), new LoggerFactory())
                .CreateMapper();
            _mockUow = new Mock<IUnitOfWork>();
            _categoryService = new CategoryService(_mockUow.Object, _mapper);
            _mockRepo = new Mock<ICategoryRepository>();
        }
        [Fact]
        public async Task CreateCategoryAsync_WhenCreatingSuccessfully_ReturnsSuccessResult()
        {
            var model = new CreateUpdateCategoryDto
            {
                Name = "Test-Category"
            };

            _mockRepo.Setup(r => r.IsNameExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(false);
            _mockUow.Setup(u => u.Categories).Returns(_mockRepo.Object);

            var result = await _categoryService.CreateCategoryAsync(model, new CancellationToken());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            _mockUow.Verify
                (u => u.Categories.CreateAsync(It.IsAny<Category>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
        [Fact]
        public async Task CreateCategory_DuplicateName_ReturnsConflict()
        {
            var model = new CreateUpdateCategoryDto
            {
                Name = "Test-Category"
            };
            _mockRepo.Setup(r => r.IsNameExist(model.Name, It.IsAny<CancellationToken>()))
               .ReturnsAsync(true);
            _mockUow.Setup(u => u.Categories).Returns(_mockRepo.Object);

            var result = await _categoryService.CreateCategoryAsync(model, It.IsAny<CancellationToken>());

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.Conflict, result.ErrorType); 
            _mockUow.Verify
                (u => u.Categories.CreateAsync(It.IsAny<Category>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }
        [Fact]
        public async Task UpdateCategory_EntityNotFound_ReturnsNotFound()
        {
            var model = new CreateUpdateCategoryDto
            {
                CategoryId = 1,
                Name = "Test-Category"
            };
            _mockRepo.Setup(r => r.GetByIdAsync(model.CategoryId.Value, It.IsAny<CancellationToken>() , true))
               .ReturnsAsync((Category)null!);
            _mockUow.Setup(u => u.Categories).Returns(_mockRepo.Object);

            var result = await _categoryService.UpdateCategoryAsync(model.CategoryId.Value , model, It.IsAny<CancellationToken>());

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);

            _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }
        [Fact]

        public async Task UpdateCategory_SameName_DoesNotCheckDatabase()
        {
            var model = new CreateUpdateCategoryDto
            {
                CategoryId = 1,
                Name = "Test-Category"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(model.CategoryId.Value, It.IsAny<CancellationToken>(), true))
               .ReturnsAsync(new Category (model.Name));
            _mockUow.Setup(u => u.Categories).Returns(_mockRepo.Object);

            var result = await _categoryService.UpdateCategoryAsync(model.CategoryId.Value, model, It.IsAny<CancellationToken>());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            _mockRepo.Verify(r => r.IsNameExist(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

    }
}
