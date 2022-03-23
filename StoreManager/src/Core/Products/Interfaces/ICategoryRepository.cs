using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Products.Models;

namespace Core.Products.Interfaces;

public interface ICategoryRepository
{
    public Task<CategoryResponse> CreateCategoryAsync(CategoryRequest categoryRequest);
    public Task<CategoryResponse> UpdateCategoryAsync(CategoryUpdatedRequest categoryUpdatedRequest);
    public Task<CategoryResponse> GetCategoryByIdAsync(int id);
    public Task<bool> DeleteCategoryByIdAsync(int id);
    public Task<IEnumerable<CategoryResponse>> ListCategoriesAsync();
}