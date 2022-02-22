using Bogus;
using Core.Users.Models;

namespace Dummie.Test.Users;

public sealed class RoleRequestDummie : Faker<RoleRequest>
{
    public RoleRequestDummie()
    {
        RuleFor(x => x.Name, f => f.Person.FullName);
        RuleFor(x => x.IsAdmin, f => f.Random.Bool());
    }
}