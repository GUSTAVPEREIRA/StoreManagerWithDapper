using System.Threading.Tasks;
using Core.Products;
using Core.Products.Interfaces;
using Dapper;
using Infrastructure.Providers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Products;

public class CategoryRepository : BaseRepository, ICategoryRepository
{
    private const string InsertCategoryQuery = @"INSERT INTO categories 
        (name) VALUES (@name) RETURNING ID";

    public CategoryRepository(IConfiguration configuration, IDbConnectionProvider provider) : base(configuration,
        provider)
    {
    }

    public async Task<Category> CreateRoleAsync(Category category)
    {
        await using var connection = GetConnection();

        var id = await connection.ExecuteScalarAsync<int>(InsertCategoryQuery, new
        {
            name = category.Name
        });

        category.Id = id;

        return category;
    }
}