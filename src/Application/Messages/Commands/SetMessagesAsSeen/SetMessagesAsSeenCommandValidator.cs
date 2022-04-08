using FluentValidation;

namespace talker.Application.Messages.Commands.SetMessagesAsSeen
{
    public class SetMessagesAsSeenCommandValidator : AbstractValidator<SetMessagesAsSeenCommand>
    {
        public SetMessagesAsSeenCommandValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty()
                .WithMessage("ConversationId is required.");

            RuleFor(x => x.MessagesIds)
                .NotEmpty()
                .Must(m => m.Length > 0)
                .WithMessage("As least one message Id is required.");

            RuleForEach(x => x.MessagesIds)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Message Id is invalid.");

        }
    }
}
