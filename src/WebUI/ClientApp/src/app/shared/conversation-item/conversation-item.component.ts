import { DialogConfirmation } from './../dialogs/dialog-confirmation/dialog-confirmation';
import { ConversationsClient } from './../../web-api-client';
import { Component, EventEmitter, Input, OnInit, Output, OnChanges, SimpleChanges, OnDestroy } from '@angular/core';
import { NG_ASYNC_VALIDATORS } from '@angular/forms';
import * as moment from 'moment';
import { UserClientCacheService } from 'src/app/services/user-client-cache.service';
import { ApplicationUserDto, ConversationDto } from 'src/app/web-api-client';
import { Helpers } from '../Constants/Helpers';
import { IMouseOptionsItem } from '../mouse-menu/imouse-options-item';
import { MatDialog } from '@angular/material/dialog';

@Component({
    selector: 'app-conversation-item',
    templateUrl: './conversation-item.component.html',
    styleUrls: ['./conversation-item.component.scss']
})
export class ConversationItemComponent implements OnInit, OnChanges, OnDestroy {

    @Output() onItemClicked: EventEmitter<number> =
        new EventEmitter<number>();

    @Input() convId: number;

    @Input() pics: string[] = [];
    @Input() title: string = "";
    @Input() lastMessage: string | undefined;
    @Input() lastMessageTime: Date | undefined;
    @Input() displayName: string | undefined;
    @Input() unseenMessages: number | undefined;

    unseenFormatted: string;
    dateFormatted: string;

    showOptions = false;


    optionsItems: IMouseOptionsItem[] = [
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
    ]

    constructor(private conversationsClient: ConversationsClient,
        public dialog: MatDialog) {
    }


    displayItem() {
        let unsen = this.unseenMessages;

        if (unsen > 0) {
            unsen = Helpers.clamp(unsen, 1, 10);
            this.unseenFormatted = `${unsen > 9 ? "9+" : unsen}`;
        }

        this.dateFormatted = this.formatDate(this.lastMessageTime);
    }

    intervalId: any;

    ngOnInit(): void {
        moment.locale('en');

        moment.updateLocale('en', {
            relativeTime: {
                future: "in %s",
                past: "%s ago",
                s: 'now',
                ss: '%d seconds',
                m: "1 minute",
                mm: "%d minutes",
                h: "1 hour",
                hh: "%d hours",
                d: "1 day",
                dd: "%d days",
                w: "1 week",
                ww: "%d weeks",
                M: "1 month",
                MM: "%d months",
                y: "1 year",
                yy: "%d years"
            }
        });

        this.displayItem();
        this.intervalId = setInterval(() => {
            this.dateFormatted = this.formatDate(this.lastMessageTime);
        }, 20 * 1000)
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.displayItem();
    }


    onItemClick() {
        this.onItemClicked.emit(this.convId);
    }

    formatDate(date: any) {

        return moment(date).fromNow(true);
    }

    onMenuShown(event) {
        this.showOptions = event;
    }

    onMenuOptionClick(val: number) {
        switch (val) {
            case 0:
                this.conversationsClient.markAsRead(this.convId).subscribe();
                break;
            case 1: {
                const dialogRef = this.dialog.open(DialogConfirmation,{
                        data: {
                            text: "Are you sure you want to leave the conversation?",
                            type: 'confirm'
                        }
                    }
                );
                dialogRef.afterClosed().subscribe(result =>{
                    if(result)
                        this.conversationsClient.leaveConversation(this.convId).subscribe();
                })
            }
                break;
        }
    }

    ngOnDestroy(): void {
        if (this.intervalId) {
            clearInterval(this.intervalId);
        }
    }

}
