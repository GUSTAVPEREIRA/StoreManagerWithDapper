using Bogus;
using Core.Users;

namespace Dummies.Test.Users
{
    public class RoleDummie : Faker<Role>
    {
        public RoleDummie()
        {
            RuleFor(x => x.Name, f => new Faker().Random.String(5, 100));
            RuleFor(x => x.IsAdmin, f => f.Random.Bool());
        }
    }
}