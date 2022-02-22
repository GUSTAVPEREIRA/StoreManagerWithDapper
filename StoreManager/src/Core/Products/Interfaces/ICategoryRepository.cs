using System.Threading.Tasks;

namespace Core.Products.Interfaces;

public interface ICategoryRepository
{
    public Task<Category> CreateRoleAsync(Category category);
}