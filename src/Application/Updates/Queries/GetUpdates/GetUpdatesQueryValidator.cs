using FluentValidation;


namespace talker.Application.Updates.Queries.GetUpdates
{
    class GetUpdatesQueryValidator : AbstractValidator<GetUpdatesQuery>
    {
        public GetUpdatesQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User Id is required.");

            RuleFor(x => x.TimeOfLastEvnet)
                .NotEmpty().WithMessage("Time of last evnet is required.");
        }
    }
}
