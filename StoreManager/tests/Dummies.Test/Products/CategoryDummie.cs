using Bogus;
using Core.Products;
using Infrastructure.Products.Models;

namespace Dummie.Test.Products;

public sealed class CategoryDummie : Faker<Category>
{
    public CategoryDummie()
    {
        RuleFor(x => x.Id, x => x.Random.Int());
        RuleFor(x => x.Name, x => x.Name.FullName());
    }
}