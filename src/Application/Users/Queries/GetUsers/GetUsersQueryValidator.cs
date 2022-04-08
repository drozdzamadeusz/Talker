using FluentValidation;

namespace talker.Application.Users.Queries.GetUsers
{
    public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        public GetUsersQueryValidator()
        {
            RuleFor(x => x.UserIds)
                .Must(x => x.Count > 0)
                .WithMessage("Pass at leat one id");
        }
    }
}