using AutoMapper;
using InventoryManagement.Application.DTO;
using InventoryManagement.Application.ResultHelpers;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateProductDTO, Product>();
            CreateMap<Product, ProductDetailsDTO>().
                ForMember(dest => dest.CategoryName, p => p.MapFrom(src => src.Category.Name)).
                ForMember(dest => dest.ProfitPerUnit, p => p.MapFrom(src => src.ProfitPerUnit()));
            CreateMap<Product, ProductListDTO>().
                ForMember(dest => dest.CategoryName, p => p.MapFrom(src => src.Category.Name)); 
        }
    }
}
