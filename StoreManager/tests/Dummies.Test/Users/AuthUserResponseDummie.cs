using Bogus;
using Core.Users.Models;

namespace Dummie.Test.Users;

public sealed class AuthUserResponseDummie : Faker<AuthUserResponse>
{
    public AuthUserResponseDummie()
    {
        RuleFor(x => x.Disabled, x => x.Random.Bool());
        RuleFor(x => x.Email, x => x.Person.Email);
        RuleFor(x => x.Id, x => x.Random.Int(1, 9999));
        RuleFor(x => x.Password, x => x.Hacker.Phrase());
        RuleFor(x => x.Role, new RoleResponseDummie().Generate());
        RuleFor(x => x.FullName, x => x.Person.FullName);
    }
}