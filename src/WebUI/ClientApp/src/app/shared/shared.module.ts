import { LoadingSpinnerComponent } from './loading-spinner/loading-spinner.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConversationItemComponent } from './conversation-item/conversation-item.component';
import { UserItemComponent } from './user-item/user-item.component';
import { UserSelectItemComponent } from './user-select-item/user-select-item.component';
import { DialogConfirmation } from './dialogs/dialog-confirmation/dialog-confirmation';
import { RouterModule } from '@angular/router';
import { WindowComponent } from './windows/window/window.component';
import { NotepadWindowComponent } from './windows/notepad-window/notepad-window.component';
import { MouseMenuComponent } from './mouse-menu/mouse-menu.component';
import { AvatarComponent } from './avatar/avatar.component';
import { UserListPicsArrayPipe } from './pipes/user-list-pics-array.pipe';
import { UserIdUserDisplayNamePipe } from './pipes/user-id-user-display-name.pipe';
import { UserIdsToUserDtosPipe } from './pipes/user-ids-to-user-dtos.pipe';
import { UserIdsListToArrayPipe } from './pipes/user-ids-list-to-array.pipe';
import { ToArrayPipe } from './pipes/to-array.pipe';
import { DialogConversationUsersComponent } from './dialogs/dialog-conversation-users/dialog-conversation-users.component';
import { DialogAddUserToConversationComponent } from './dialogs/dialog-add-user-to-conversation/dialog-add-user-to-conversation.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DialogInputComponent } from './dialogs/dialog-input/dialog-input.component';
import { DialogColorSelectComponent } from './dialogs/dialog-color-select/dialog-color-select.component';
import { DialogMessageDetails } from './dialogs/dialog-message-details/dialog-message-details.component';
import { ToThemeClassColorPipe } from './pipes/to-theme-class-color.pipe';
import { MatDialogModule } from '@angular/material/dialog';



@NgModule({
  declarations: [
    ConversationItemComponent,
    UserItemComponent,
    UserSelectItemComponent,
    DialogConfirmation,
    LoadingSpinnerComponent,
    WindowComponent,
    NotepadWindowComponent,
    MouseMenuComponent,
    AvatarComponent,
    UserListPicsArrayPipe,
    UserIdUserDisplayNamePipe,
    UserIdsToUserDtosPipe,
    UserIdsListToArrayPipe,
    ToArrayPipe,
    DialogConversationUsersComponent,
    DialogAddUserToConversationComponent,
    DialogInputComponent,
    DialogColorSelectComponent,
    DialogMessageDetails,
    ToThemeClassColorPipe,
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    MatDialogModule
  ],
  exports:[
    ConversationItemComponent,
    UserItemComponent,
    UserSelectItemComponent,
    DialogConfirmation,
    LoadingSpinnerComponent,
    WindowComponent,
    NotepadWindowComponent,
    MouseMenuComponent,
    AvatarComponent,
    UserListPicsArrayPipe,
    UserIdUserDisplayNamePipe,
    UserIdsToUserDtosPipe,
    UserIdsListToArrayPipe,
    ToArrayPipe,
    DialogConversationUsersComponent,
    DialogAddUserToConversationComponent,
    DialogInputComponent,
    ToThemeClassColorPipe,
  ]
})
export class SharedModule { }
