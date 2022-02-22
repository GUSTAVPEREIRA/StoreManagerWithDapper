using Bogus;
using Core.Products;

namespace Dummie.Test.Products;

public sealed class CategoryDummie : Faker<Category>
{
    public CategoryDummie()
    {
        RuleFor(x => x.Id, x => x.Random.Int());
        RuleFor(x => x.Name, x => x.Name.FullName());
    }
}