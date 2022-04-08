import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IConfirmationDialogData } from './Iconfirmation-dialog-data';


@Component({
  selector: 'app-dialog-confirmation',
  templateUrl: './dialog-confirmation.component.html',
  styleUrls: ['./dialog-confirmation.component.scss']
})
export class DialogConfirmation implements OnInit {

  constructor(public dialog: MatDialog,
    private dialogRef: MatDialogRef<DialogConfirmation>,
    @Inject(MAT_DIALOG_DATA) public data: IConfirmationDialogData) {
      if(data.type == 'error' && !data.text) data.text = "Unknown error occurred";
      if(data.type == 'confirm' && !data.text) data.text = "Are you sure you want to continue?";
    }

  ngOnInit(): void {
  }

  onConfirmClicked() {
    this.dialogRef.close(true);
  }

  onCloseClicked() {
    this.dialogRef.close();
  }

}
