import { ApplicationUserDto } from 'src/app/web-api-client';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
    selector: 'app-user-select-item',
    templateUrl: './user-select-item.component.html',
    styleUrls: ['./user-select-item.component.scss']
})
export class UserSelectItemComponent implements OnInit {

    @Input() user: ApplicationUserDto;
    @Input() controls:boolean = true;

    @Output() onItemClicked: EventEmitter<ApplicationUserDto> = 
    new EventEmitter<ApplicationUserDto>();

    constructor() { }

    ngOnInit(): void {
    }

    onItemClick(){
        this.onItemClicked.emit(this.user);
    }

}
