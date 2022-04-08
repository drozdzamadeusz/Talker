using System;
using System.Collections.Generic;
using System.Linq;
using talker.Domain.Entities;

namespace talker.Application.Common.Helpers
{
    public static class HelperMethods
    {
        public static bool CheckIfUserBelongsToConversation(IList<UserConversation> usersIds, string userId)
            => usersIds.Where(u => u.UserId == userId && u.IsDeleted == false).Count() > 0;

        public static DateTime TrimMilliseconds(DateTime dateTime)
            => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0, dateTime.Kind);

        public static bool CheckIfDateOccuredBefore(DateTime firstDate, DateTime secondDate)
            => TrimMilliseconds(firstDate) < TrimMilliseconds(secondDate);
    }
}
