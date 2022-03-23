using Bogus;
using Core.Products.Models;

namespace Dummie.Test.Products;

public sealed class CategoryResponseDummie : Faker<CategoryResponse>
{
    public CategoryResponseDummie()
    {
        RuleFor(x => x.Id, x => x.Random.Int(1, 999));
        RuleFor(x => x.Name, x => x.Person.FullName);
    }
}