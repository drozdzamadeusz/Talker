import { BrowserModule } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Component, ElementRef, Input, OnInit, ViewChild, OnDestroy, OnChanges, SimpleChanges, AfterContentInit, HostListener } from '@angular/core';
import { ConversationColor, ConversationsClient, IMessageDto, MessageDto, MessagesClient } from 'src/app/web-api-client';
import { UserClientCacheService } from 'src/app/services/user-client-cache.service';
import { EchoService } from 'src/app/services/echo.service';

@Component({
    selector: 'app-messages',
    templateUrl: './messages.component.html',
    styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit, OnDestroy, OnChanges, AfterContentInit {

    ConversationColor = ConversationColor;

    @Input() conversationId: number;
    @Input() color: ConversationColor = ConversationColor.Classic;


    private lastMessageNumber = 0;
    public messages: MessageDto[];

    currentUserId?: string | undefined;

    private messagesClientSubscription: any;
    private userClientCacheServiceSubscription: any;
    private echoServiceSubscription: any;

    @ViewChild('messagesContainer') private messagesContainer: ElementRef;

    constructor(private userClientCacheService: UserClientCacheService,
        private messagesClient: MessagesClient,
        private echoService: EchoService) { }

    ngOnInit(): void {
        this.userClientCacheServiceSubscription = this.userClientCacheService.getCurrentUser().subscribe(
            currentUser => {
                let { id: currentUserId } = currentUser;
                this.currentUserId = currentUserId;
            }
        );

        this.messagesClientSubscription = this.messagesClient.getMessages(this.conversationId, this.lastMessageNumber, 40, false).subscribe(
            result => {
                this.messages = result.messages;

                this.setMessagesAsSeen(this.messages, this.conversationId);

                this.sortMessages();

                if(!this.messages || !this.messages[0] || !this.messages[0].id)
                    return;

                this.topMessageId = this.messages[0].id;
            }
        );

        this.echoServiceSubscription = this.echoService.messagesUpdate.subscribe(
            data => {
                if (!data || !data.messages || !this.messages)
                    return;

                let newMessages = data.messages.filter(
                    m => m.conversationId === this.conversationId && this.messages.filter(o => o.id === m.id).length < 1);

                if (newMessages) {
                    newMessages = newMessages.sort((a, b) => a.id - b.id);
                    newMessages.forEach(m => this.messages.push(new MessageDto(m)));

                    this.setMessagesAsSeen(newMessages, this.conversationId);
                }
            }
        );

    }

    setMessagesAsSeen(messages: IMessageDto[], conversationId: number) {
        let listOfMessagestoSetAsSeen = messages.filter(
            m => m.createdBy !== this.currentUserId &&
                m.seenBy.filter(
                    s => s.userId === this.currentUserId
                ).length < 1)
            .map(m => m.id);
        this.echoService.setMessagesAsSeen(listOfMessagestoSetAsSeen, conversationId);
    }

    private lastMessage: number;
    private topMessageId: number = 0;

    private fetchingOldMessages: boolean = false;
    private fetchedOldMessagesAllMessages: boolean = false;
    private scrollAfterOldMesssagesLoaded: boolean = false;

    private featchOldMessages(lastMessageId: number) {

        this.messagesClientSubscription = this.messagesClient.getMessages(this.conversationId, lastMessageId, 40, true).subscribe(
            result => {

                if (!result || !result.messages)
                    return;

                if (result.messages.length < 1) {
                    this.fetchingOldMessages = false;
                    this.fetchedOldMessagesAllMessages = true;
                    return;
                }

                this.messages = [ ...result.messages, ...this.messages];

                this.setMessagesAsSeen(this.messages, this.conversationId);

                this.sortMessages();

                this.topMessageId = this.messages[0].id ?? 0;

                this.scrollAfterOldMesssagesLoaded = true;
            },
            error =>{
                this.fetchingOldMessages = false;
                this.fetchedOldMessagesAllMessages = true;
            }
        );
    }

    private userScrolling = false;

    private lastScrollHeight: number;

    @HostListener('scroll', ['$event'])
    onScroll(event: any) {

        let scrolledTop = event.target.scrollTop < 6;

        if (event.target.scrollHeight - (event.target.offsetHeight + event.target.scrollTop) >= 50) {
            this.userScrolling = true;
        }

        if (event.target.offsetHeight + event.target.scrollTop >= event.target.scrollHeight) {
            this.userScrolling = false;
        }

        if (scrolledTop && !this.fetchingOldMessages && !this.fetchedOldMessagesAllMessages && this.topMessageId > 0) {
            this.fetchingOldMessages = true;
            this.lastScrollHeight = this.messagesContainer.nativeElement.scrollHeight;
            this.featchOldMessages(this.topMessageId);
        }
    }


    ngOnChanges(changes: SimpleChanges): void {
    }

    ngAfterContentInit(): void {
    }

    messageLoadedEvent(messageId: number) {
        if (!this.userScrolling && !this.fetchingOldMessages) {
            this.scrollToBottom();
        } else if (this.userScrolling && this.scrollAfterOldMesssagesLoaded && messageId === this.topMessageId) {
            this.scrollAfterOldMesssagesLoaded = false;
            // setTimeout(() =>{
                let scrollDif = this.messagesContainer.nativeElement.scrollHeight - this.lastScrollHeight;

                this.messagesContainer.nativeElement.scrollTop += scrollDif;

                this.lastScrollHeight = this.messagesContainer.nativeElement.scrollHeight;
                //setTimeout(() =>{
                this.fetchingOldMessages = false;
                //}, 1000);
            // },500);
        }
    }

    sortMessages() {
        this.messages = this.messages.sort((a, b) => a.id - b.id);
    }

    scrollToBottom(): void {
        setTimeout(() => {
            try {
                this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
            } catch (err) {
            }
        }, 0);
    }

    ngOnDestroy(): void {
        if (this.messagesClientSubscription)
            this.messagesClientSubscription.unsubscribe();

        if (this.userClientCacheServiceSubscription)
            this.userClientCacheServiceSubscription.unsubscribe();


        if (this.echoServiceSubscription)
            this.echoServiceSubscription.unsubscribe();
    }
}
