import { ConversationRole } from "src/app/web-api-client";
import { IMouseOptionsItem } from "../../mouse-menu/imouse-options-item";

export interface DialogConversationUsersDto {
    id: string,
    text: string,
    description: string,
    pictureUrl: string,
    role: ConversationRole,
    options: IMouseOptionsItem[],
}
