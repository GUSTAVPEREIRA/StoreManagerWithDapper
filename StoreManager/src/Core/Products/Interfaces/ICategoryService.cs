using System.Threading.Tasks;
using Core.Products.Models;

namespace Core.Products.Interfaces;

public interface ICategoryService
{
    public Task<CategoryResponse> CreateCategoryAsync(CategoryRequest categoryRequest);
}