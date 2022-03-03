using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Products;
using Core.Products.Interfaces;
using Dummie.Test.Products;

namespace Repository.Test.Seeders;

public class CategorySeeder
{
    private readonly ICategoryRepository _categoryRepository;

    public CategorySeeder(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<List<Category>> CreateCategories(int count)
    {
        var categories = new CategoryDummie().Generate(count);

        return await InsertCategories(categories);
    }

    private async Task<List<Category>> InsertCategories(List<Category> categories)
    {
        foreach (var category in categories)
        {
            await _categoryRepository.CreateCategoryAsync(category);
        }

        return categories;
    }
}