import { ConversationsClient, CreateConversationCommand, UeserIdDto } from './../web-api-client';
import { Component, OnInit } from '@angular/core';
import { ApplicationUserDto, UsersClient } from '../web-api-client';

import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { UserClientCacheService } from '../services/user-client-cache.service';
import { EchoService } from '../services/echo.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-new-conversation',
    templateUrl: './new-conversation.component.html',
    styleUrls: ['./new-conversation.component.scss']
})
export class NewConversationComponent implements OnInit {

    public searchResult: ApplicationUserDto[] = [];
    queryField: FormControl = new FormControl();

    public searchText: string;
    public selectedUsers: ApplicationUserDto[] = [];
    public currentUser: ApplicationUserDto;

    private conversationName: string | undefined;

    constructor(private userClientCacheService: UserClientCacheService,
        private userClient: UsersClient,
        private conversationsClient: ConversationsClient,
        public echoService: EchoService,
        private router: Router) {
    }

    ngOnInit(): void {
        this.userClientCacheService.getCurrentUser().subscribe(
            result => {
                this.currentUser = result;
            }
        );

        this.queryField.valueChanges
            .pipe(debounceTime(400),
                distinctUntilChanged()
            )
            .subscribe(queryField => {
                this.searchText = queryField;
                this.searchUsers();
            }
        );
    }

    searchUsers() {
        let firstName = "";
        let lastName = "";
        let userName = "";

        if (this.searchText.includes('"')) {
            userName = this.searchText.split('"')[1].trim();
            this.searchText = this.searchText.replace(/\"[^)]*\"/g, "");
            this.searchText = this.searchText.replace(/ +(?= )/g, "");
        }
        if (this.searchText.includes(' ')) {
            firstName = this.searchText.split(' ')[0].trim();
            lastName = this.searchText.split(' ')[1].trim();
        } else {
            firstName = this.searchText.trim();
        }

        console.log(`firstName: ${firstName} lastName: ${lastName} userName: ${userName}`);

        this.userClient.findUsers(userName, firstName, lastName).subscribe(
            result => {
                this.searchResult = result.filter((newObjs) => !this.selectedUsers.find(o => o.id === newObjs.id) && newObjs.id !== this.currentUser.id);
            }
        )
    }

    onUserItemClicked(userId: string) {
        this.selectedUsers.push(this.searchResult.find(u => u.id === userId));
        this.searchResult = this.searchResult.filter(item => item.id !== userId);
    }

    onSelectedUserItemClicked(user: ApplicationUserDto) {
        this.selectedUsers = this.selectedUsers.filter(item => item.id !== user.id);
        this.searchUsers();
    }


    onConversationNameChanged(event) {
        this.conversationName = event.target.value;
    }


    createNewConversation() {
        if (this.selectedUsers.length < 1) {
            alert("Select at least one user");
            return;
        }

        let conversationCommand: CreateConversationCommand = new CreateConversationCommand();

        if (this.conversationName)
            conversationCommand.name = this.conversationName;
        this.selectedUsers.forEach(u => {
            if (!conversationCommand.usersIds)
                conversationCommand.usersIds = [];
            conversationCommand.usersIds.push(new UeserIdDto({
                userId: u.id
            }));

        });

        this.conversationsClient.createConversation(conversationCommand).subscribe(
            result => {
                this.echoService.connection
                    .invoke('AddToHub', result)
                    .catch(
                        error => {}
                    );
                this.router.navigate(['/start']);
            }
        )
    }
}

