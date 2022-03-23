using AutoMapper;
using Core.Products.Models;
using Infrastructure.Products.Models;

namespace Infrastructure.Products.Mappings;

public class CategoryMappingProfile: Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CategoryRequest, Category>().ReverseMap();
        CreateMap<CategoryUpdatedRequest, CategoryResponse>().ReverseMap();
        CreateMap<CategoryRequest, CategoryResponse>().ReverseMap();
        CreateMap<CategoryRequest, CategoryUpdatedRequest>().ReverseMap();
        CreateMap<Category, CategoryResponse>().ReverseMap();
        
        
    }
}