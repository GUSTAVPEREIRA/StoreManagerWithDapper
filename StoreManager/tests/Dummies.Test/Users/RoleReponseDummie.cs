using Bogus;
using Core.Users.Models;

namespace Dummie.Test.Users
{
    public sealed class RoleReponseDummie : Faker<RoleResponse>
    {
        public RoleReponseDummie()
        {
            RuleFor(x => x.Id, f => f.Random.Int(1, 9999));
            RuleFor(x => x.Name, f => f.Person.FullName);
            RuleFor(x => x.IsAdmin, f => f.Random.Bool());
        }
    }
}