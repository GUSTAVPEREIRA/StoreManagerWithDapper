using Bogus;
using Core.Users.Models;

namespace Dummie.Test.Users;

public sealed class UserUpdateRequestDummie : Faker<UserUpdatedRequest>
{
    public UserUpdateRequestDummie()
    {
        RuleFor(x => x.Disabled, false);
        RuleFor(x => x.Id, x => x.Random.Int());
        RuleFor(x => x.Email, x => x.Person.Email);
        RuleFor(x => x.Password, x => x.Lorem.Word());
        RuleFor(x => x.FullName, x => x.Person.FullName);
        RuleFor(x => x.RoleId, x => x.Random.Int());
    }
}