using System.Linq;
using talker.Domain.Entities;

namespace talker.Application.Common.Helpers
{
    public static class HelperMethods
    {
        public static bool CheckIfUserBelongsToConversation(Conversation conversation, string userId)
            => conversation.UsersIds.Where(u => u.UserId == userId && u.UserRemoved == false).Count() > 0;
    }
}
