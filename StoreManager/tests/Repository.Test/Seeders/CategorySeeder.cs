using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Products.Interfaces;
using Core.Products.Models;
using Dummie.Test.Products;

namespace Repository.Test.Seeders;

public class CategorySeeder
{
    private readonly ICategoryRepository _categoryRepository;

    public CategorySeeder(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryResponse>> CreateCategories(int count)
    {
        var categories = new CategoryRequestDummie().Generate(count);

        return await InsertCategories(categories);
    }

    private async Task<List<CategoryResponse>> InsertCategories(List<CategoryRequest> categories)
    {
        var categoryResponses = new List<CategoryResponse>();

        foreach (var category in categories)
        {
            categoryResponses.Add(await _categoryRepository.CreateCategoryAsync(category));
        }

        return categoryResponses;
    }
}