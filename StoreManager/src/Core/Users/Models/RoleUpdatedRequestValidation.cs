using FluentValidation;

namespace Core.Users.Models
{
    public class RoleUpdatedRequestValidation : AbstractValidator<RoleUpdatedRequest>
    {
        public RoleUpdatedRequestValidation()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().GreaterThan(0);
            Include(new RoleRequestValidation());
        }
    }
}