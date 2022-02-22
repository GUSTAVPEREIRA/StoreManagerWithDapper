using FluentValidation;

namespace Core.Users.Models;

public class RoleRequestValidation : AbstractValidator<RoleRequest>
{
    public RoleRequestValidation()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull().MinimumLength(5).MaximumLength(100);
    }
}