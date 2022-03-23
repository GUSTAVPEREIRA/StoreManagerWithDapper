using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Products.Interfaces;
using Core.Products.Models;
using Dapper;
using Infrastructure.Products.Models;
using Infrastructure.Providers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Products;

public class CategoryRepository : BaseRepository, ICategoryRepository
{
    private readonly IMapper _mapper;

    private const string InsertCategoryQuery = @"INSERT INTO categories 
        (name) VALUES (@name) RETURNING ID";

    private const string UpdateCategoryQuery = @"UPDATE categories SET name=@name
        WHERE id=@id";

    private const string GetCategoryByIdQuery = @"SELECT * FROM categories 
        WHERE id=@id";

    private const string ListCategoriesQuery = @"SELECT * FROM categories";

    private const string DeleteCategoryByIdquery = @"DELETE FROM categories WHERE id=@id";

    public CategoryRepository(IConfiguration configuration, IDbConnectionProvider provider, IMapper mapper) : base(
        configuration,
        provider)
    {
        _mapper = mapper;
    }

    public async Task<CategoryResponse> CreateCategoryAsync(CategoryRequest categoryRequest)
    {
        await using var connection = GetConnection();

        var id = await connection.ExecuteScalarAsync<int>(InsertCategoryQuery, new
        {
            name = categoryRequest.Name
        });

        if (id > 0)
        {
            var categoryResponse = _mapper.Map<CategoryRequest, CategoryResponse>(categoryRequest);
            categoryResponse.Id = id;
            return categoryResponse;
        }

        return null;
    }

    public async Task<CategoryResponse> UpdateCategoryAsync(CategoryUpdatedRequest categoryUpdatedRequest)
    {
        await using var connection = GetConnection();
        var updated = await connection.ExecuteAsync(UpdateCategoryQuery, new
        {
            name = categoryUpdatedRequest.Name,
            id = categoryUpdatedRequest.Id
        });

        var categoryResponse = _mapper.Map<CategoryUpdatedRequest, CategoryResponse>(categoryUpdatedRequest);

        return updated > 0 ? categoryResponse : null;
    }

    public async Task<CategoryResponse> GetCategoryByIdAsync(int id)
    {
        await using var connection = GetConnection();

        var category = await connection.QueryFirstOrDefaultAsync<Category>(GetCategoryByIdQuery, new
        {
            id
        });

        var t = _mapper.Map<Category, CategoryResponse>(category);
        return t;
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

    public async Task<IEnumerable<CategoryResponse>> ListCategoriesAsync()
    {
        await using var connection = GetConnection();

        var categories = await connection.QueryAsync<Category>(ListCategoriesQuery);

        return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResponse>>(categories);
    }
}