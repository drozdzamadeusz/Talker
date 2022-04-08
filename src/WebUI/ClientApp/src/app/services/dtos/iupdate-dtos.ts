import { ConversationColor, IConversationDetailedDto, IConversationDto, IMessageDto } from "src/app/web-api-client";

export interface IMessageUpdateDto {
    messages?: IMessageDto[] | undefined;
    now?: Date;
}


export interface IConversationUpdateDto {
    type?: EConversationUpdateType;
    id?: number | undefined;
    unseenMessages?: number | undefined;
    userId?: string | undefined;
    name?: string | undefined;
    color?: ConversationColor | undefined;
    now?: Date;
}

export enum EUserUpdateType{
    LeftConversation,
    RemovedFromConversation,
    AddedToConversation,
}

export enum EConversationUpdateType {
    UserMarkedMessagesAsRead,
    UserLeftConversation,
    UserRemovedFromConversation,
    UserAddedToConversation,
    AdminGranted,
    AdminRevoked,
    ConversationNameChanged,
    ConversationColorChanged,
}

export interface IUserUpdateDto {
    type?: EUserUpdateType,
    conversationId?: number,
    content?: string,
    now?: Date;
}