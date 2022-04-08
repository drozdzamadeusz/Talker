import { ApplicationUserDto } from "src/app/web-api-client";

export interface DialogAddUserToConversationDto {
    selectedUsers: ApplicationUserDto[],
    currentUserId: string;
}
