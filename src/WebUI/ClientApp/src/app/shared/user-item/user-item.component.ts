import { ApplicationUserDto } from './../../web-api-client';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
    selector: 'app-user-item',
    templateUrl: './user-item.component.html',
    styleUrls: ['./user-item.component.scss']
})
export class UserItemComponent implements OnInit {

    @Output() onItemClicked: EventEmitter<string> = 
                new EventEmitter<string>();

    @Input() userId: string;
    @Input() pictureUrl: string;
    @Input() text: string;
    @Input() description: string;

    constructor() { }

    ngOnInit(): void {
    }

    onItemClick(){
        this.onItemClicked.emit(this.userId);
    }

}
