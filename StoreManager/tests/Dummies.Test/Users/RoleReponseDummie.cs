using Bogus;
using Core.Users.Models;

namespace Dummie.Test.Users;

public sealed class RoleReponseDummie : Faker<RoleResponse>
{
    public RoleReponseDummie(int id = 0)
    {
        var newId = id == 0 ? new Faker().Random.Int(1, 9999) : id;
        RuleFor(x => x.Id, newId);
        RuleFor(x => x.Name, f => f.Person.FullName);
        RuleFor(x => x.IsAdmin, f => f.Random.Bool());
    }
}