import { UserClientCacheService } from 'src/app/services/user-client-cache.service';
import { ApplicationUserDto, MessageType } from 'src/app/web-api-client';
import { MessageDto } from './../../../web-api-client';
import { Component, Input, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DialogMessageDetails } from 'src/app/shared/dialogs/dialog-message-details/dialog-message-details.component';
import { Helpers } from 'src/app/shared/Constants/Helpers';

@Component({
    selector: 'app-message',
    templateUrl: './message.component.html',
    styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit, OnDestroy {

    MessageType: MessageType;

    @Input() message: MessageDto;
    @Input() mine: boolean;
    @Input() type: MessageType = MessageType.Normal;

    @Output() messageLoadedEvent = new EventEmitter<number>();

    user: ApplicationUserDto;
    private sub: any;

    constructor(private userClientCacheService: UserClientCacheService,
        public dialog: MatDialog) { }

    ngOnInit(): void {
        this.sub = this.userClientCacheService.getUser(`${this.message.createdBy}`).subscribe(result => {
            this.user = result;
            this.displayMessage();
        });
    }

    displayMessage() {
        this.messageLoadedEvent.emit(this.message.id);
    }

    showDetails(event: any) {

        let { clientX: x, clientY: y } = event;

        let size = {
            width: '340px',
            maxHeight: '400px'
        }

        let { innerWidth, innerHeight } = window;
        let top = Helpers.clamp(y - 190, 0, innerHeight - 400);
        let left = Helpers.clamp(x - 170, 0, innerWidth - 340);
        let pos = {
            position: {
                top: `${top}px`,
                left: `${left}px`
            }
        }

        const dialogRef = this.dialog.open(DialogMessageDetails, {
            data: {
                messageId: this.message.id
            },
            ...pos,
            ...size
        });
    }

    ngOnDestroy(): void {
        if (this.sub)
            this.sub.unsubscribe();
    }
}
