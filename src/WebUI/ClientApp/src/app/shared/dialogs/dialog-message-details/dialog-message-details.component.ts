import { MessageDto } from './../../../web-api-client';
import { MessagesClient, ApplicationUserDto } from 'src/app/web-api-client';
import { DialogMessageDetailsDto } from './dialog-message-details-dto';
import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserClientCacheService } from 'src/app/services/user-client-cache.service';


interface MessageDetailsApplicationUserDto {
    id?: string | undefined;
    firstName?: string | undefined;
    lastName?: string | undefined;
    pictureUrl?: string | undefined;
    created?: Date;
}

@Component({
    selector: 'app-dialog-message-details',
    templateUrl: './dialog-message-details.component.html',
    styleUrls: ['./dialog-message-details.component.scss']
})
export class DialogMessageDetails implements OnInit, OnDestroy{

    private messageServiceSubscription: any;

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: DialogMessageDetailsDto,
        public dialogRef: MatDialogRef<any>,
        private messagesClient: MessagesClient,
        private userClientCacheService: UserClientCacheService) {
    }

    message: MessageDto;
    users: MessageDetailsApplicationUserDto[];
    creator: ApplicationUserDto;

    ngOnInit(): void {
        this.messageServiceSubscription = this.messagesClient.getMessage(this.data.messageId).subscribe(
            message =>{
                this.message = message;

                let ids = message.seenBy.map(m => m.userId);

                this.userClientCacheService.convertIdsToUsers(ids).subscribe(
                    users =>{
                        this.users = users;
                        
                        this.users = users.map(u =>{
                            let created = this.message.seenBy.find(s => s.userId === u.id).created;

                            return{
                                id: u.id,
                                firstName: u.firstName,
                                lastName: u.lastName,
                                pictureUrl: u.pictureUrl,
                                created
                            }
                        });

                    }
                );

                this.userClientCacheService.getUser(message.createdBy).subscribe(
                    creator =>{
                        this.creator = creator;
                    }
                )
            }
        );
    }

    copyToClipboard(): void {
        const selBox = document.createElement("textarea");
        selBox.style.position = "fixed";
        selBox.style.left = "0";
        selBox.style.top = "0";
        selBox.style.opacity = "0";
        selBox.value = this.message.content;
        document.body.appendChild(selBox);
        selBox.focus();
        selBox.select();
        document.execCommand("copy");
        document.body.removeChild(selBox);
      }

    ngOnDestroy(): void {
        if (this.messageServiceSubscription)
            this.messageServiceSubscription.unsubscribe();
    }

    onClose(){
        this.dialogRef.close();
    }

}
