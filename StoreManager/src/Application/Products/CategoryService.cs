using System.Threading.Tasks;
using Core.Products.Interfaces;
using Core.Products.Models;

namespace Application.Products;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }


    public Task<CategoryResponse> CreateCategoryAsync(CategoryRequest categoryRequest)
    {
        return _categoryRepository.CreateCategoryAsync(categoryRequest);
    }
}