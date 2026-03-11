using InventoryManagement.Application.Persistence;
using InventoryManagement.Application.Persistence.Repositories;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Infrastructure
{
    public static class  DependencyInjection
    {
        public static IServiceCollection AddUnitOfWorkAndRepository(
        this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Unit of Work 
            services.AddScoped<IUnitOfWork, UnitOfWork>();  
            return services; 
        }
    }
}
