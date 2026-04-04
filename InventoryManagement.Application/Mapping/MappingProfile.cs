using AutoMapper;
using InventoryManagement.Shared.DTO;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagement.Application.DTOS;

namespace InventoryManagement.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<Category, CategoryListDTO>(); 
            CreateMap<CreateProductApiDto, Product>();
            CreateMap<Product, ProductDetailsDTO>().
                ForMember(dest => dest.CategoryName, p => p.MapFrom(src => src.Category.Name)).
                ForMember(dest => dest.ProfitPerUnit, p => p.MapFrom(src => src.ProfitPerUnit()));
            CreateMap<Product, ProductListDTO>().
                ForMember(dest => dest.CategoryName, p => p.MapFrom(src => src.Category.Name)); 
           
            CreateMap<Warehouse, WarehouseDTO>().
                ForMember(dest => dest.NameWithCity, p => p.MapFrom(src => $"{src.Name}-({src.City})"));
            CreateMap<Warehouse, WarehouseDetailsDTO>();

        }
    }
}
