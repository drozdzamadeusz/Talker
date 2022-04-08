import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DialogInputDto } from './dialog-input-dto-data';

@Component({
    selector: 'app-dialog-input',
    templateUrl: './dialog-input.component.html',
    styleUrls: ['./dialog-input.component.scss']
})
export class DialogInputComponent implements OnInit {

    constructor(
        public dialogRef: MatDialogRef<string>,
        @Inject(MAT_DIALOG_DATA) public data: DialogInputDto) { }

    ngOnInit(): void {
    }

    onClose(){
        this.dialogRef.close();
    }

    onSave(){
        this.dialogRef.close(this.data.text);
    }

}
