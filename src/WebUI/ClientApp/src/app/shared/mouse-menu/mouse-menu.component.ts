import { IMouseOptionsItem } from './imouse-options-item';
import { Component, HostListener, Input, OnInit, EventEmitter, Output } from '@angular/core';


@Component({
    selector: 'app-mouse-menu',
    templateUrl: './mouse-menu.component.html',
    styleUrls: ['./mouse-menu.component.scss']
})
export class MouseMenuComponent implements OnInit {

    @Input() showOptions = false;
    @Input() options: IMouseOptionsItem[] = []

    @Output() onMenuShown = new EventEmitter<boolean>();
    @Output() onOpionClicked = new EventEmitter<number>();

    constructor() { }

    ngOnInit(): void {
    }

    showOptionsPanel(event){
        this.showOptions = !this.showOptions;
        this.onMenuShown.emit(this.showOptions);
        event.stopPropagation();
    }

    
    onControlButtonClick(event: any, index: number){
        event.preventDefault();

        this.onOpionClicked.emit(index);
        this.hideOptionsPanel(null);
        event.stopPropagation();
    }

    @HostListener('document:click', ['$event'])
    hideOptionsPanel(event:any){
        this.showOptions = false;
        this.onMenuShown.emit(this.showOptions);
    }

}
