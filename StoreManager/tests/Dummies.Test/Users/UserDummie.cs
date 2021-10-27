using Bogus;
using Core.Users;

namespace Dummie.Test.Users
{
    public sealed class UserDummie : Faker<User>
    {
        public UserDummie()
        {
            RuleFor(x => x.Id, f => f.Random.Int());
            RuleFor(x => x.Role, new RoleDummie().Generate());
            RuleFor(x => x.FullName, f => f.Person.FullName);
        }
    }
}