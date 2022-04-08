import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IMouseOptionsItem } from '../../mouse-menu/imouse-options-item';
import { DialogConversationUsersDto } from './dialog-conversation-users-dto';
import { DialogResponseWithIndexDto } from '../dialog-response-with-index-dto';

@Component({
    selector: 'app-dialog-conversation-users',
    templateUrl: './dialog-conversation-users.component.html',
    styleUrls: ['./dialog-conversation-users.component.scss']
})

export class DialogConversationUsersComponent implements OnInit {

    constructor(
        public dialogRef: MatDialogRef<DialogResponseWithIndexDto>,
        @Inject(MAT_DIALOG_DATA) public data: DialogConversationUsersDto[]) { }

    ngOnInit(): void {

    }

    onMenuOptionClick(index: number, userId: string){
        let result: DialogResponseWithIndexDto = {
            index,
            userId,
        }
        this.dialogRef.close(result);
    }

    onClose(){
        this.dialogRef.close();
    }

}
