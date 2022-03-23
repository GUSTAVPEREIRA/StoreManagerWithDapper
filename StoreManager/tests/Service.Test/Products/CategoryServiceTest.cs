using System.Threading.Tasks;
using Application.Products;
using Core.Products.Interfaces;
using Core.Products.Models;
using Dummie.Test.Products;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Service.Test.Products;

public class CategoryServiceTest
{
    private readonly ICategoryService _categoryService;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryServiceTest()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _categoryService = new CategoryService(_categoryRepository);
    }

    [Fact]
    public async Task CreateCategoryAsyncOk()
    {
        var categoryResponse = new CategoryResponseDummie().Generate();
        var categoryRequest = new CategoryRequest
        {
            Name = categoryResponse.Name
        };

        _categoryRepository.CreateCategoryAsync(Arg.Any<CategoryRequest>()).Returns(categoryResponse);

        var result = await _categoryService.CreateCategoryAsync(categoryRequest);
        categoryResponse.Id = result.Id;

        result.Should().BeEquivalentTo(categoryResponse);
    }
}