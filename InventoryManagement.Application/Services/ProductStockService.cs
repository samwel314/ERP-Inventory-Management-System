using AutoMapper;
using AutoMapper.QueryableExtensions;
using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services
{
    public class ProductStockService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;

        public ProductStockService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<ProductStockDto>>> GetAllAsync( int ? warehouseId = null , string ? searchTerm = null  , CancellationToken ct = default)
        {
            var productStockQuery =  _db.ProductStocks.GetAll();
            if (warehouseId != null)
                productStockQuery = productStockQuery.Where(ps => ps.WarehouseId == warehouseId);
            if (searchTerm != null)
            {
                var cleanSearch = searchTerm.Trim();
                productStockQuery = productStockQuery.
                   Where(ps => ps.Product.Name.Contains(cleanSearch) || ps.Product.SKU.Contains(cleanSearch));
            }
            productStockQuery = productStockQuery.OrderByDescending(ps => ps.LastUpdated);
            var productStock = await productStockQuery.ProjectTo< ProductStockDto >(_mapper.ConfigurationProvider)
                 .ToListAsync(ct);
            return Result<IEnumerable<ProductStockDto>>.Success(productStock);
        }
    }

}


