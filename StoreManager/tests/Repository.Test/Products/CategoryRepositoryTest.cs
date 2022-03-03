using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Products.Interfaces;
using Dummie.Test.Products;
using FluentAssertions;
using Infrastructure.Products;
using Repository.Test.Configuration;
using Repository.Test.Seeders;
using Xunit;

namespace Repository.Test.Products;

public class CategoryRepositoryTest : IDisposable
{
    private const string DatabaseName = "categoryDatabase";
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategorySeeder _categorySeeder;

    public CategoryRepositoryTest()
    {
        var configuration = new RepositoryTestConfiguration().CreateConfigurations(DatabaseName);
        DatabaseConfiguration.CreateMigrations(DatabaseName);
        _categoryRepository = new CategoryRepository(configuration, new SqLiteDbConnectionProvider());
        _categorySeeder = new CategorySeeder(_categoryRepository);
    }

    [Fact]
    public async Task InsertCategoryOk()
    {
        var category = new CategoryDummie().Generate();
        var response = await _categoryRepository.CreateCategoryAsync(category);

        response.Should().BeEquivalentTo(category);
    }

    [Fact]
    public async Task UpdateCategoryOk()
    {
        var categories = await _categorySeeder.CreateCategories(1);
        var category = categories.First();
        var oldName = category.Name;
        category.Name = "ChangedName";

        var updatedResponse = await _categoryRepository.UpdateCategoryAsync(category);

        updatedResponse.Name.Should().NotBeEquivalentTo(oldName);
    }

    [Fact]
    public async Task GetCategoryOk()
    {
        var categories = await _categorySeeder.CreateCategories(1);
        var category = categories.First();

        var response = await _categoryRepository.GetCategoryByIdAsync(category.Id);

        category.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task DeleteCategoryOk()
    {
        var categories = await _categorySeeder.CreateCategories(1);
        var category = categories.First();

        var response = await _categoryRepository.DeleteCategoryByIdAsync(category.Id);

        response.Should().Be(true);
    }   
    
    [Fact]
    public async Task DeleteCategoryWithoutRowAffected()
    {
        var response = await _categoryRepository.DeleteCategoryByIdAsync(1);

        response.Should().Be(false);
    }

    [Fact]
    public async Task ListCategoriesOk()
    {
        var categories = await _categorySeeder.CreateCategories(10);

        var response = await _categoryRepository.ListCategoriesAsync();

        response.ToList().Should().BeEquivalentTo(categories);
    }

    public void Dispose()
    {
        DatabaseConfiguration.RemoveMigrations(DatabaseName);
    }
}