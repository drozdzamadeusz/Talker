
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConversationColor } from 'src/app/web-api-client';
import { DialogInputDto } from '../dialog-input/dialog-input-dto-data';
import { DialogColorSelectDataDto } from './dialog-color-select-data-dto';

export interface color {
    id: number,
    name: string,
    color: ConversationColor,

}

@Component({
    selector: 'app-dialog-color-select',
    templateUrl: './dialog-color-select.component.html',
    styleUrls: ['./dialog-color-select.component.scss']
})

export class DialogColorSelectComponent implements OnInit {

    public colors: color[];

    constructor(
        public dialogRef: MatDialogRef<number>,
        @Inject(MAT_DIALOG_DATA) public data: DialogColorSelectDataDto) { }

    ngOnInit(): void {

        let colors: color[] = [];
        
        for(const value in ConversationColor){
            if (typeof ConversationColor[value] !== "string") {
                continue;
            }
            let id = Number(value);
            let name = ConversationColor[value];
            let color = ConversationColor[ConversationColor[value]]
            colors.push({
                id, name, color
            });
        }

        this.colors = colors;

    }

    onClose() {
        this.dialogRef.close();
    }


    onChangeActive(id: number) {
        this.data.selectedId = id;
    }

    onSave() {
        this.dialogRef.close(this.data.selectedId);
    }

}
