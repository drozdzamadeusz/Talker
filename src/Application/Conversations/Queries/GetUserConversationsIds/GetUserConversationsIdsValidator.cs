using FluentValidation;

namespace talker.Application.Conversations.Queries.GetUserConversationsIds
{
    public class GetUserConversationsIdsValidator: AbstractValidator<GetUserConversationsIdsQuery>
    {
        public GetUserConversationsIdsValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
