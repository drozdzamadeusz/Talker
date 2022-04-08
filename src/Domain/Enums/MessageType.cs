namespace talker.Domain.Enums
{
    public enum MessageType
    {
        Normal = 0,
        UserLeftConversation,
        RemovedUserFromConversation,
        AddedUserToConversation,
        AdminGranted,
        AdminRevoked,
        ConversationNameChanged,
        ConversationColorChanged,
    }
}
