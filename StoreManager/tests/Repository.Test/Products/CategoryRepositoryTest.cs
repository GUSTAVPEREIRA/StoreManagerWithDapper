using System;
using System.Threading.Tasks;
using Core.Products.Interfaces;
using Dummie.Test.Products;
using FluentAssertions;
using Infrastructure.Products;
using Repository.Test.Configuration;
using Xunit;

namespace Repository.Test.Products;

public class CategoryRepositoryTest : IDisposable
{
    private const string DatabaseName = "categoryDatabase";
    private readonly ICategoryRepository _categoryRepository;

    public CategoryRepositoryTest()
    {
        var configuration = new RepositoryTestConfiguration().CreateConfigurations(DatabaseName);
        DatabaseConfiguration.CreateMigrations(DatabaseName);
        _categoryRepository = new CategoryRepository(configuration, new SqLiteDbConnectionProvider());
    }

    [Fact]
    public async Task InsertCategoryOk()
    {
        var category = new CategoryDummie().Generate();
        var response = await _categoryRepository.CreateRoleAsync(category);

        category.Id = 1;

        response.Should().BeEquivalentTo(category);
    }

    public void Dispose()
    {
        DatabaseConfiguration.RemoveMigrations(DatabaseName);
    }
}