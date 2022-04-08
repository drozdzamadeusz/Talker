using FluentValidation;

namespace talker.Application.Messages.Queries.GetMessagesWithPagination
{
    public class GetMessagesWithPaginationQueryValidator : AbstractValidator<GetMessagesWithPaginationQuery>
    {
        public GetMessagesWithPaginationQueryValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty().WithMessage("ConversationId is required.");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
        }
    }
}
