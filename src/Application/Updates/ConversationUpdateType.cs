namespace talker.Application.Updates
{
    public enum ConversationUpdateType
    {
        UserMarkedMessagesAsRead = 0,
        UserLeftConversation,
        UserRemovedFromConversation,
        UserAddedToConversation,
        AdminGranted,
        AdminRevoked,
        ConversationNameChanged,
        ConversationColorChanged,
    }
}
