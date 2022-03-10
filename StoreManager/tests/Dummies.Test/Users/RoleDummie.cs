using Bogus;
using Core.Users;
using Infrastructure.Users.Models;

namespace Dummie.Test.Users;

public sealed class RoleDummie : Faker<Role>
{
    public RoleDummie()
    {
        RuleFor(x => x.Name, f => f.Name.FullName());
        RuleFor(x => x.IsAdmin, f => f.Random.Bool());
    }
}