using System.Collections.Generic;
using System.Linq;
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

    private const string UpdateCategoryQuery = @"UPDATE categories SET name=@name
        WHERE id=@id";

    private const string GetCategoryByIdQuery = @"SELECT * FROM categories 
        WHERE id=@id";

    private const string ListCategoriesQuery = @"SELECT * FROM categories";

    private const string DeleteCategoryByIdquery = @"DELETE FROM categories WHERE id=@id";

    public CategoryRepository(IConfiguration configuration, IDbConnectionProvider provider) : base(configuration,
        provider)
    {
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        await using var connection = GetConnection();

        var id = await connection.ExecuteScalarAsync<int>(InsertCategoryQuery, new
        {
            name = category.Name
        });

        category.Id = id;

        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        await using var connection = GetConnection();
        var updated = await connection.ExecuteAsync(UpdateCategoryQuery, new
        {
            name = category.Name,
            id = category.Id
        });

        return updated > 0 ? category : null;
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        await using var connection = GetConnection();

        var category = await connection.QueryFirstOrDefaultAsync<Category>(GetCategoryByIdQuery, new
        {
            id
        });

        return category;
    }

    public async Task<bool> DeleteCategoryByIdAsync(int id)
    {
        await using var connection = GetConnection();

        var isDeleted = await connection.ExecuteAsync(DeleteCategoryByIdquery, new
        {
            id
        });

        return isDeleted > 0;
    }

    public async Task<IEnumerable<Category>> ListCategoriesAsync()
    {
        await using var connection = GetConnection();

        var categories = await connection.QueryAsync<Category>(ListCategoriesQuery);

        return categories;
    }
}