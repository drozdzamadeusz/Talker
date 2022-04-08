import { Component, Inject, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ApplicationUserDto, UsersClient } from 'src/app/web-api-client';
import { DialogResponseWithIndexDto } from '../dialog-response-with-index-dto';
import { DialogConversationUsersDto } from '../dialog-conversation-users/dialog-conversation-users-dto';
import { DialogAddUserToConversationDto } from './dialog-add-user-to-conversation-dto';

@Component({
    selector: 'app-dialog-add-user-to-conversation',
    templateUrl: './dialog-add-user-to-conversation.component.html',
    styleUrls: ['./dialog-add-user-to-conversation.component.scss']
})
export class DialogAddUserToConversationComponent implements OnInit {

    public searchResult: ApplicationUserDto[] = [];

    queryField: FormControl = new FormControl();

    public searchText: string;

    constructor(
        private userClient: UsersClient,
        public dialogRef: MatDialogRef<DialogResponseWithIndexDto>,
        @Inject(MAT_DIALOG_DATA) public data: DialogAddUserToConversationDto) {
    }

    ngOnInit(): void {

        this.queryField.valueChanges
            .pipe(debounceTime(400),
                distinctUntilChanged()
            )
            .subscribe(queryField => {
                this.searchText = queryField;
                this.searchUsers();
            }
        );
    }

    searchUsers() {
        let firstName = "";
        let lastName = "";
        let userName = "";

        if (this.searchText.includes('"')) {
            userName = this.searchText.split('"')[1].trim();
            this.searchText = this.searchText.replace(/\"[^)]*\"/g, "");
            this.searchText = this.searchText.replace(/ +(?= )/g, "");
        }
        if (this.searchText.includes(' ')) {
            firstName = this.searchText.split(' ')[0].trim();
            lastName = this.searchText.split(' ')[1].trim();
        } else {
            firstName = this.searchText.trim();
        }

        console.log(`firstName: ${firstName} lastName: ${lastName} userName: ${userName}`);

        this.userClient.findUsers(userName, firstName, lastName).subscribe(
            result => {
                this.searchResult = result.filter((newObjs) => !this.data.selectedUsers.find(o => o.id === newObjs.id) && newObjs.id !== this.data.currentUserId);
            }
        )
    }


    onUserItemClicked(userId: string){
        let result: DialogResponseWithIndexDto = {
            index: 9,
            userId,
        }
        this.dialogRef.close(result);
    }

    onClose(){
        this.dialogRef.close();
    }

}
