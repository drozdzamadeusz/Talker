import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { IWindowInfo } from '../../dtos/iwindow-info';
import { WindowsService } from '../../services/windows.service';

@Component({
    selector: 'app-notepad-window',
    templateUrl: './notepad-window.component.html',
    styleUrls: ['./notepad-window.component.scss']
})
export class NotepadWindowComponent implements OnInit,OnDestroy{

    @Input() public windowHandler: IWindowInfo;

    lastSavedVal: string;
    textAreaVal: string = "";
    saveInterval: any;

    constructor(private windowService: WindowsService) { }

    ngOnInit(): void {
        this.lastSavedVal = this.windowService.getContent(this.windowHandler.id);
        this.textAreaVal = this.lastSavedVal;

        this.saveInterval = setInterval(() =>{
            if(this.lastSavedVal !== this.textAreaVal){
                console.info("Saving note...");
                this.windowService.setContent(this.windowHandler.id, this.textAreaVal);
                this.lastSavedVal = this.textAreaVal;
            }
        }, 5000);
    }

    ngOnDestroy(): void {
        if(this.saveInterval)
            clearInterval(this.saveInterval);
    }
}
