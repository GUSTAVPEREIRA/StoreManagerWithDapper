using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Products.Interfaces;

public interface ICategoryRepository
{
    public Task<Category> CreateCategoryAsync(Category category);
    public Task<Category> UpdateCategoryAsync(Category category);
    public Task<Category> GetCategoryByIdAsync(int id);
    public Task<bool> DeleteCategoryByIdAsync(int id);
    public Task<IEnumerable<Category>> ListCategoriesAsync();
}