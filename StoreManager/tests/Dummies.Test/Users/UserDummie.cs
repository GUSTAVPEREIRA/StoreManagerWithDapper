using Bogus;
using Core.Users;

namespace Dummie.Test.Users;

public sealed class UserDummie : Faker<User>
{
    public UserDummie()
    {
        RuleFor(x => x.Role, new RoleDummie().Generate());
        CreateRule();
    }

    public UserDummie(Role role)
    {
        RuleFor(x => x.Role, role);
        CreateRule();
    }

    private void CreateRule()
    {
        RuleFor(x => x.Id, f => f.Random.Int());
        RuleFor(x => x.FullName, f => f.Person.FullName);
        RuleFor(x => x.Email, f => f.Person.Email);
        RuleFor(x => x.Password, f => f.Random.String2(10, 100));
        RuleFor(x => x.Disabled, f => f.Random.Bool());
    }
}