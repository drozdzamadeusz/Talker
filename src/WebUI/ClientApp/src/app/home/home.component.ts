import { WindowsService } from '../shared/services/windows.service';
import { EchoService } from './../services/echo.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationUserDto, ConversationDetailedDto, ConversationDto, ConversationRole, ConversationsClient, MessageDto, UserConversationDto } from '../web-api-client';
import { Helpers } from '../shared/Constants/Helpers';
import { Constants } from '../shared/Constants/Constants';
import * as _ from 'underscore';
import { UserClientCacheService } from '../services/user-client-cache.service';
import { EWindowState } from '../shared/dtos/ewindow-state';
import { IWindowInfo } from '../shared/dtos/iwindow-info';
import { EWindowType } from '../shared/dtos/ewindow-type';
import { IMouseOptionsItem } from '../shared/mouse-menu/imouse-options-item';
import { Router } from '@angular/router';
import { EConversationUpdateType, EUserUpdateType } from '../services/dtos/iupdate-dtos';
import { MatDialog } from '@angular/material/dialog';


@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {

    EWindowType = EWindowType;

    public currentUser: ApplicationUserDto;

    public users: ApplicationUserDto[] = [];

    public userConversations: ConversationDetailedDto[];

    public conversationWindows: IWindowInfo[] = [];

    private echoServiceSubscription: any;

    optionsItems: IMouseOptionsItem[] = [
        {
            index: 0,
            text: "Profile",
            iconClass: "fas fa-user"
        },
        {
            index: 2,
            text: "Notepad",
            iconClass: "fas fa-sticky-note"
        },
        {
            index: 1,
            text: "Logout",
            iconClass: "fas fa-sign-out-alt"
        },
    ]

    constructor(public userClientCacheService: UserClientCacheService,
        public conversationsClient: ConversationsClient,
        public echoService: EchoService,
        private windowsService: WindowsService,
        private router: Router,
        public dialog: MatDialog) { }


    ngOnInit(): void {
        this.userClientCacheService.getCurrentUser().subscribe(
            result => {
                this.currentUser = result;
            }
        );

        this.getConversations();

        this.windowsService.windowsListChanged.subscribe(
            windows => {
                if (!windows)
                    return;

                this.conversationWindows = windows.filter(w => w.id != 0);

                this.getUsers();
            }
        );


        this.subscribeEchoServer();
    }

    getConversations() {
        this.conversationsClient.getUserConversations().subscribe(
            result => {

                result = result.sort(this.sorter).reverse();
                this.userConversations = result;
                this.getUsers();
            }
        );
    }

    getUsers() {
        if (!this.userConversations)
            return;

        let ids: Set<string> = new Set();
        this.userConversations.forEach(c =>
            c.users.forEach(u =>
                ids.add(u.userId)
            )
        )

        this.userClientCacheService.convertIdsToUsers(Array.from(ids)).subscribe(
            result => {
                this.users = result;
            }
        );
    }


    sorter(a: ConversationDetailedDto, b: ConversationDetailedDto) {
        let first = a.lastMessage?.created;
        if (!first)
            first = a.created

        let second = b.lastMessage?.created;
        if (!second)
            second = b.created;
        return new Date(first).getTime() - new Date(second).getTime();
    }


    subscribeEchoServer() {
        this.echoServiceSubscription = this.echoService.messagesUpdate.subscribe((updateData) => {

            if (!updateData)
                return;

            let updatedMessages = updateData?.messages;

            if (updatedMessages.length > 0 && this.userConversations) {
                this.userConversations.forEach(conv => {

                    let updatedMessage = updatedMessages.find(el => el.conversationId === conv.id)

                    if (!updatedMessage)
                        return;

                    if (updatedMessage.createdBy !== this.currentUser.id)
                        conv.unseenMessages = conv.unseenMessages + 1;

                    if (conv.lastMessage) {
                        conv.lastMessage = _.extend(conv.lastMessage, updatedMessage);
                    } else {
                        conv.lastMessage = new MessageDto(updatedMessage);
                    }

                });
                this.userConversations = this.userConversations.sort(this.sorter).reverse();
            }
        });



        this.echoServiceSubscription = this.echoService.conversationsUpdate.subscribe((update) => {

            if (!update || !this.userConversations)
                return;

            let updatedConv = this.userConversations.find(c => c.id === update.id);

            if (!updatedConv)
                return;

            switch (update.type) {
                case EConversationUpdateType.UserMarkedMessagesAsRead: {
                    updatedConv.unseenMessages = update.unseenMessages;
                }
                    break;
                case EConversationUpdateType.ConversationNameChanged: {
                    updatedConv.name = update.name;
                }
                    break;
                case EConversationUpdateType.ConversationColorChanged: {
                    updatedConv.color = update.color;
                }
                    break;
                case EConversationUpdateType.UserLeftConversation: {
                    updatedConv.users = updatedConv.users.filter(u => u.userId !== update.userId);
                }
                    break;
                case EConversationUpdateType.UserRemovedFromConversation: {
                    updatedConv.users = updatedConv.users.filter(u => u.userId !== update.userId);
                }
                    break;
                case EConversationUpdateType.UserAddedToConversation: {
                    if (updatedConv.users.find(u => u.userId === update.userId)) {
                        break;
                    }
                    this.userConversations.find(c => c.id === update.id).users.push(new UserConversationDto({
                        userId: update.userId,
                        role: ConversationRole.User,
                    }));
                    this.getUsers();
                }
                    break;
                default:
                    break;
            }

        });

        this.echoServiceSubscription = this.echoService.userUpdate.subscribe((data) => {

            if (!data)
                return;

            if (data.type === EUserUpdateType.LeftConversation || data.type === EUserUpdateType.RemovedFromConversation) {
                this.userConversations = this.userConversations.filter(c => c.id != data.conversationId);
            }


            if (data.type === EUserUpdateType.AddedToConversation) {
                this.getConversations();
            }

        });
    }

    onConversationItemClicked(conversationId: number) {
        let openedWindow = this.conversationWindows.find(o => o.id === conversationId);

        if (openedWindow) {
            this.windowsService.moveToTop(openedWindow.id);
            return;
        }

        let { innerWidth, innerHeight } = window;
        let { width, height } = Constants.DEFAULT_WINDOW_DIMENSIONS;

        let rect = {
            x: Math.max(Helpers.clamp(Helpers.randomInteger(0, innerWidth - width), 0, innerWidth - width), 0),
            y: Math.max(Helpers.clamp(Helpers.randomInteger(0, innerHeight - height), 0, innerHeight - height), 0),
            width: Helpers.clamp(width, 0, innerWidth),
            height: Helpers.clamp(height, 0, innerHeight)
        }


        this.windowsService.openNewWindow({
            id: conversationId,
            type: EWindowType.CONVERSATION,
            state: EWindowState.WINDOWED,
            rect,
            order: 0,
            moveToTop: false
        });
    }

    onMenuShown(event) {
    }

    onMenuOptionClick(val: number) {
        switch (val) {
            case 0:
                this.router.navigate(["/authentication/profile"]);
                break;
            case 1:
                this.router.navigate(["/authentication/logout"], { state: { local: true } });
                break;
            case 2:
                this.openNotepad();
                break;
        }
    }

    openNotepad() {
        let { innerWidth, innerHeight } = window;
        let width = 300;
        let height = 400;

        let rect = {
            x: Math.max(Helpers.clamp(Helpers.randomInteger(0, innerWidth - width), 0, innerWidth - width), 0),
            y: Math.max(Helpers.clamp(Helpers.randomInteger(0, innerHeight - height), 0, innerHeight - height), 0),
            width: Helpers.clamp(width, 0, innerWidth),
            height: Helpers.clamp(height, 0, innerHeight)
        }

        this.windowsService.openNewWindow({
            type: EWindowType.NOTEPAD,
            state: EWindowState.WINDOWED,
            rect,
            order: 0,
            moveToTop: false
        });
    }

    ngOnDestroy(): void {
        if (this.echoServiceSubscription)
            this.echoServiceSubscription.unsubscribe();
    }
}
