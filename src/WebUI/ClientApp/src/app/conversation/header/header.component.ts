import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { UserClientCacheService } from 'src/app/services/user-client-cache.service';
import { ApplicationUserDto, ConversationDto } from 'src/app/web-api-client';

@Component({
  selector: 'app-conversation-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit{

    @Input() text: string;
    @Input() pics: string[];

    ngOnInit(): void {
    }
}
