import { IRect } from './../../shared/dtos/irect';
import { map, delay, retry, retryWhen, tap } from 'rxjs/operators';
import { DialogConversationUsersDto } from './../../shared/dialogs/dialog-conversation-users/dialog-conversation-users-dto';
import { DialogConversationUsersComponent } from './../../shared/dialogs/dialog-conversation-users/dialog-conversation-users.component';
import { EchoService } from './../../services/echo.service';
import { Component, EventEmitter, Input, OnInit, Output, OnDestroy, ViewChild, ElementRef, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EWindowState } from 'src/app/shared/dtos/ewindow-state';
import { IWindowInfo } from 'src/app/shared/dtos/iwindow-info';
import { ITopControls } from 'src/app/shared/windows/window/window.component';
import { ConversationDto, ConversationsClient, ISendMessageCommand, MessagesClient, SendMessageCommand, ApplicationUserDto, ConversationRole, UpdateConversationCommand, UserConversationDto, ConversationColor } from 'src/app/web-api-client';
import { IMouseOptionsItem } from 'src/app/shared/mouse-menu/imouse-options-item';
import { UserClientCacheService } from 'src/app/services/user-client-cache.service';
import { MatDialog } from '@angular/material/dialog';
import { DialogAddUserToConversationDto } from 'src/app/shared/dialogs/dialog-add-user-to-conversation/dialog-add-user-to-conversation-dto';
import { DialogAddUserToConversationComponent } from 'src/app/shared/dialogs/dialog-add-user-to-conversation/dialog-add-user-to-conversation.component';
import { DialogInputComponent } from 'src/app/shared/dialogs/dialog-input/dialog-input.component';
import { EConversationUpdateType } from 'src/app/services/dtos/iupdate-dtos';
import { DialogColorSelectComponent } from 'src/app/shared/dialogs/dialog-color-select/dialog-color-select.component';
import { ICords } from 'src/app/shared/dtos/icords';
import { Helpers } from 'src/app/shared/Constants/Helpers';
import { DialogConfirmation } from 'src/app/shared/dialogs/dialog-confirmation/dialog-confirmation';

@Component({
    selector: 'app-conversation-window',
    templateUrl: './conversation-window.component.html',
    styleUrls: ['./conversation-window.component.scss']
})
export class ConversationWindowComponent implements OnInit, OnDestroy {

    private routeSubscription: any;
    private echoServiceSubscription: any;
    private userClientCacheServiceSubscription: any;

    ITopControls = ITopControls;
    EWindowState = EWindowState;
    ConversationColor = ConversationColor;

    @Input() public windowHandler: IWindowInfo;
    @Input() public conversationId: number;

    conversation: ConversationDto;
    users: ApplicationUserDto[];
    currentUserId?: string | undefined;

    currentUserRole: ConversationRole | undefined;


    @Output() windowStateChanged: EventEmitter<IWindowInfo> =
        new EventEmitter<IWindowInfo>();

    textareaVal = "";


    optionsItems: IMouseOptionsItem[] = [];


    constructor(public userClientCacheService: UserClientCacheService,
        private conversationsClient: ConversationsClient,
        private echoService: EchoService,
        private messagesClient: MessagesClient,
        public dialog: MatDialog) {
    }

    numberOfRetries = 0;

    ngOnInit(): void {
        if (this.conversationId != 0) {
            this.conversationsClient.getConversation(this.conversationId).pipe(
                retryWhen(
                    errors => errors.pipe(
                        delay(2000),
                        tap((error) => {
                            if(this.numberOfRetries >= 10){
                                throw error;
                            }
                            this.numberOfRetries++;
                        })
                    )
                )
            ).subscribe(result => {
                this.conversation = result;

                this.userClientCacheServiceSubscription = this.userClientCacheService.getCurrentUser()
                    .subscribe(
                        currentUser => {
                            let { id: currentUserId } = currentUser;
                            this.currentUserId = currentUserId;
                            this.currentUserRole = this.conversation.users.find(cUser => cUser.userId === this.currentUserId).role ?? ConversationRole.User;

                            this.setOptions(this.currentUserRole);
                        }
                    );

                let ids = this.conversation.users.map(u => u.userId);
                this.getUsers(ids);
            });
        }

        this.echoServiceSubscription = this.echoService.conversationsUpdate.subscribe((update) => {
            if (!update)
                return;

            if (update.id !== this.conversationId || !this.conversation || !this.conversation.users)
                return;

            // try{
            if (update.type === EConversationUpdateType.ConversationNameChanged) {
                this.conversation.name = update.name;
            } else if (update.type === EConversationUpdateType.ConversationColorChanged) {
                this.conversation.color = update.color;
            }

            switch (update.type) {
                case EConversationUpdateType.ConversationNameChanged: {
                    this.conversation.name = update.name;
                }
                    break;
                case EConversationUpdateType.ConversationColorChanged: {
                    this.conversation.color = update.color;
                }
                    break;
                case EConversationUpdateType.UserLeftConversation: {
                    this.conversation.users = this.conversation.users.filter(u => u.userId !== update.userId);
                    this.users = this.users.filter(u => u.id !== update.userId);
                }
                    break;
                case EConversationUpdateType.UserRemovedFromConversation: {
                    this.conversation.users = this.conversation.users.filter(u => u.userId !== update.userId);
                    this.users = this.users.filter(u => u.id !== update.userId);
                }
                    break;
                case EConversationUpdateType.UserAddedToConversation: {
                    if (this.conversation.users.find(u => u.userId === update.userId)) {
                        break;
                    }
                    this.conversation.users.push(new UserConversationDto({
                        userId: update.userId,
                        role: ConversationRole.User,
                    }))
                    let ids = this.conversation.users.map(u => u.userId);
                    this.getUsers(ids);
                }
                    break;
                case EConversationUpdateType.AdminGranted: {
                    this.conversation.users.find(u => u.userId === update.userId).role = ConversationRole.Admin;
                    if (this.currentUserId === update.userId) {
                        this.setOptions(ConversationRole.Admin);
                    }
                }
                    break;
                case EConversationUpdateType.AdminRevoked: {
                    this.conversation.users.find(u => u.userId === update.userId).role = ConversationRole.User;
                    if (this.currentUserId === update.userId) {
                        this.setOptions(ConversationRole.User);
                    }
                }
                default:
                    console.log("Unknown update type" + update.type.toString());
                    break;
            }

            // }catch(error){
            //     console.error("Update error: "+ JSON.stringify(error));
            // }
        });
    }

    setOptions(role: ConversationRole) {
        if (role === ConversationRole.Admin || role === ConversationRole.Creator) {
            this.optionsItems = [
                {
                    index: 2,
                    text: "Group name",
                    iconClass: "fas fa-pen",
                },
                {
                    index: 3,
                    text: "Members",
                    iconClass: "fas fa-user-friends",
                },
                {
                    index: 4,
                    text: "Add user",
                    iconClass: "fas fa-user-plus",
                },
                {
                    index: 5,
                    text: "Theme",
                    iconClass: "fas fa-tint",
                },
                {
                    index: 0,
                    text: "Set as read",
                    iconClass: "fas fa-eye",
                },
                {
                    index: 1,
                    text: "Leave group",
                    iconClass: "fas fa-trash-alt",
                },
            ];
        } else {
            this.optionsItems = [
                {
                    index: 3,
                    text: "Members",
                    iconClass: "fas fa-user-friends",
                },
                {
                    index: 0,
                    text: "Set as read",
                    iconClass: "fas fa-eye",
                },
                {
                    index: 1,
                    text: "Leave group",
                    iconClass: "fas fa-trash-alt",
                },
            ];
        }
    }

    getUsers(ids: string[]) {
        this.userClientCacheService.convertIdsToUsers(Array.from(ids)).subscribe(
            result => {
                this.users = result;
            }
        );
    }

    onWindowStateChanged(window: IWindowInfo) {
        this.windowStateChanged.emit(window);
    }

    onTextareaKeydown(event) {
        if (!event.shiftKey && event.key === 'Enter') {
            event.preventDefault();
            this.sendMessage();
        } else if (event.key === 'Enter') {
        }
    }

    sendMessage() {
        if (this.textareaVal == "")
            return;

        let message: ISendMessageCommand = {
            conversationId: this.conversationId,
            content: this.textareaVal
        }

        this.textareaVal = "";
        this.messagesClient.sendMessage(new SendMessageCommand(message)).subscribe(
            result => {
                console.log("New message id: " + result);
            },
            error => {
                alert("Error while sending the message!");
            }

        );
    }

    private cursorPos: ICords;

    @HostListener('mousedown', ['$event'])
    onMousedown(event) {
        let { clientX: x, clientY: y } = event;
        this.cursorPos = {
            x, y
        }
    }

    onMenuOptionClick(val: number) {

        let size = {
            width: '360px',
            maxHeight: '500px'
        }

        let pos = null;
        if (this.cursorPos) {
            let { x, y } = this.cursorPos;
            let { innerWidth, innerHeight } = window;

            let topVal = Helpers.clamp(y - 95, 0, innerHeight - 500);
            let leftVal = Helpers.clamp(x - 180, 0, innerWidth - 360);

            let top = (topVal < 20)? {} : {top: `${topVal}px`};
            let left = (leftVal < 20)? {} : {left: `${leftVal}px`};
            
            pos = {
                position: {
                    ...top,
                    ...left
                }
            }
        }

        switch (val) {
            case 0:
                {
                    this.conversationsClient.markAsRead(this.conversationId).subscribe();
                }
                break;
            case 1:
                {
                    const dialogRef = this.dialog.open(DialogConfirmation, {
                        data: {
                            text: "Are you sure you want to leave the conversation?",
                            type: 'confirm'
                        },
                        ...pos
                    }
                    );
                    dialogRef.afterClosed().subscribe(
                        result => {
                            if (result)
                                this.conversationsClient.leaveConversation(this.conversationId).subscribe();
                        }
                    );
                }
                break;
            case 2:
                {
                    const dialogRef = this.dialog.open(DialogInputComponent, {
                        data: {
                            header: "Change group name",
                            text: this.conversation.name
                        },
                        ...pos,
                        ...size
                    });

                    dialogRef.afterClosed().subscribe(result => {
                        let conversationId = this.conversationId;

                        if (!result || result === this.conversation.name)
                            return

                        this.conversationsClient.update(conversationId, new UpdateConversationCommand({
                            id: conversationId,
                            name: result
                        })).subscribe();
                    });

                }
                break;

            case 3:
                {
                    let data: DialogConversationUsersDto[] = this.users.map(u => {
                        let role = this.conversation.users.find(cUser => cUser.userId === u.id).role;
                        if (!role) {
                            role = ConversationRole.User;
                        }

                        let roleText = "User";

                        let currentUserRole = this.conversation.users.find(cUser => cUser.userId === this.currentUserId).role ?? ConversationRole.User;

                        let options: IMouseOptionsItem[] = [];

                        if (u.id !== this.currentUserId) {
                            if (currentUserRole === ConversationRole.User) {
                                options = [];
                            } else if (currentUserRole === ConversationRole.Admin && role !== ConversationRole.Creator) {
                                options = [
                                    {
                                        index: 0,
                                        text: "Remove",
                                        iconClass: "fas fa-minus-circle",
                                    }
                                ];
                            } else if (currentUserRole === ConversationRole.Creator && (role === ConversationRole.User)) {
                                options = [
                                    {
                                        index: 0,
                                        text: "Remove",
                                        iconClass: "fas fa-minus-circle",
                                    },
                                    {
                                        index: 1,
                                        text: "Make as Admin",
                                        iconClass: "fas fa-user-plus",
                                    },
                                ];
                            } else if (currentUserRole === ConversationRole.Creator && role === ConversationRole.Admin) {
                                options = [
                                    {
                                        index: 0,
                                        text: "Remove",
                                        iconClass: "fas fa-minus-circle",
                                    },
                                    {
                                        index: 2,
                                        text: "Remove as Admin",
                                        iconClass: "fas fa-user-minus",
                                    },
                                ];
                            }
                        }

                        if (role === ConversationRole.Admin)
                            roleText = "Administrator";
                        else if (role === ConversationRole.Creator)
                            roleText = "Creator";

                        return {
                            id: u.id,
                            text: `${u.firstName} ${u.lastName}`,
                            description: `${roleText}`,
                            pictureUrl: u.pictureUrl,
                            role,
                            options
                        }
                    });

                    const dialogRef = this.dialog.open(DialogConversationUsersComponent, {
                        data,
                        ...pos,
                        ...size
                    });

                    dialogRef.afterClosed().subscribe(result => {

                        if (!result)
                            return

                        let { index, userId } = result;
                        let conversationId = this.conversationId;

                        if (!result || index === undefined || !userId)
                            return

                        switch (index) {
                            case 0:
                                this.conversationsClient.removeFromConversation(conversationId, userId).subscribe();
                                break;
                            case 1:
                                this.conversationsClient.makeUserAsAdmin(conversationId, userId).subscribe();
                                break;
                            case 2:
                                this.conversationsClient.removeUserAsAdmin(conversationId, userId).subscribe();
                                break;
                        }
                    });
                }

                break;
            case 4:
                {

                    let data: DialogAddUserToConversationDto = {
                        selectedUsers: this.users,
                        currentUserId: this.currentUserId
                    };

                    const dialogRef = this.dialog.open(DialogAddUserToConversationComponent, {
                        data,
                        ...pos,
                        ...size
                    });

                    dialogRef.afterClosed().subscribe(result => {
                        if (!result)
                            return

                        let { userId } = result;
                        let conversationId = this.conversationId;

                        if (!result || !userId)
                            return
                        this.conversationsClient.addUserConversation(conversationId, userId).subscribe();
                    });
                }
                break;
            case 5: {
                const dialogRef = this.dialog.open(DialogColorSelectComponent, {
                    data: {
                        selectedId: this.conversation.color,
                    },
                    ...pos,
                    ...size
                });

                dialogRef.afterClosed().subscribe(result => {
                    let conversationId = this.conversationId;

                    if (result === undefined || result == this.conversation.color)
                        return

                    this.conversationsClient.update(conversationId, new UpdateConversationCommand({
                        id: conversationId,
                        color: result
                    })).subscribe();

                });
            }
                break;

        }
    }

    ngOnDestroy() {
        if (this.routeSubscription)
            this.routeSubscription.unsubscribe();

        if (this.echoServiceSubscription)
            this.echoServiceSubscription.unsubscribe();
    }
}
