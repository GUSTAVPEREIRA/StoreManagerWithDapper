using Bogus;
using Core.Users.Models;

namespace Dummie.Test.Users;

public sealed class UserResponseDummie : Faker<UserResponse>
{
    public UserResponseDummie(UserRequest userRequest)
    {
        RuleFor(x => x.Disabled, false);
        RuleFor(x => x.Email, userRequest.Email);
        RuleFor(x => x.Id, x => x.Random.Int());
        RuleFor(x => x.Password, userRequest.Password);
        RuleFor(x => x.FullName, userRequest.FullName);
        RuleFor(x => x.Role, new RoleResponseDummie(userRequest.RoleId).Generate());
    }

    public UserResponseDummie()
    {
        RuleFor(x => x.Disabled, false);
        RuleFor(x => x.Email, x => x.Person.Email);
        RuleFor(x => x.Id, x => x.Random.Int());
        RuleFor(x => x.Password, x => x.Lorem.Word());
        RuleFor(x => x.FullName, x => x.Person.FullName);
        RuleFor(x => x.Role, new RoleResponseDummie().Generate());
    }
}