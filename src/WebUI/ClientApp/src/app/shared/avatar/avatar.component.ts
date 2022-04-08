import { Component, Input, OnInit } from '@angular/core';

@Component({
    selector: 'app-avatar',
    templateUrl: './avatar.component.html',
    styleUrls: ['./avatar.component.scss']
})export class AvatarComponent implements OnInit {

    @Input() pics: string[] = [];
    @Input() width: number = 55;
    @Input() height: number = 55;

    constructor() { }

    ngOnInit(): void {
    }

}
