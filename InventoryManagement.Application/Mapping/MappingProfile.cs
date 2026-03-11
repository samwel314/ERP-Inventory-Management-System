using AutoMapper;
using InventoryManagement.Application.DTO;
using InventoryManagement.Domain.Entities;
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
        }
    }
}
