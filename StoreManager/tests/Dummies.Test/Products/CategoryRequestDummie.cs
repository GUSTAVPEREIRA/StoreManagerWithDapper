using Bogus;
using Core.Products.Models;

namespace Dummie.Test.Products;

public sealed class CategoryRequestDummie : Faker<CategoryRequest>
{
    public CategoryRequestDummie()
    {
        RuleFor(x => x.Name, x => x.Person.FullName);
    }
}