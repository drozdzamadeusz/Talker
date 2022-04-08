import { WindowsService } from './../shared/services/windows.service';
import { IConversationDto } from './../web-api-client';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { EUserUpdateType, IConversationUpdateDto, IMessageUpdateDto, IUserUpdateDto } from './dtos/iupdate-dtos';



@Injectable({
    providedIn: 'root'
})
export class EchoService {

    public connection: signalR.HubConnection;

    public messagesUpdate: BehaviorSubject<IMessageUpdateDto>;
    public conversationsUpdate: BehaviorSubject<IConversationUpdateDto>;
    public userUpdate: BehaviorSubject<IUserUpdateDto>;

    private url: string = "/update";
    private jwtToken: string;
    public static NUMBER_OF_ATTEMPS = 5;

    public connectedToHub = false;

    constructor(private authorizeService: AuthorizeService,
        private windowsService: WindowsService) {
        this.messagesUpdate = new BehaviorSubject<IMessageUpdateDto>(null);
        this.conversationsUpdate = new BehaviorSubject<IConversationUpdateDto>(null);
        this.userUpdate = new BehaviorSubject<IUserUpdateDto>(null);
    }

    private async getToken(): Promise<void> {
        return new Promise((resolve, rejected) => {
            this.authorizeService.getAccessToken().subscribe(
                (t) => {
                    this.jwtToken = t;
                    resolve();
                },
                (err) => {
                    console.warn("Tocken error: " + err);
                    rejected();
                }
            );
        });
    }


    private estahlishConnection(): Promise<void> {
        return new Promise(async (resolve, reject) => {

            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(this.url, {
                    accessTokenFactory: () => this.jwtToken
                })
                .configureLogging(signalR.LogLevel.Information)
                .withAutomaticReconnect()
                .build();

            this.listenForMessagesUpdates();
            this.listenForConversationsUpdates();
            this.listenForUserUpdates();

            this.connection
                .start()
                .then(() => {
                    console.info(`Connected to the socket server.`);
                    this.connectedToHub = true;
                    resolve();
                })
                .catch((error) => {
                    console.error(`Error while starting connection: ${error}.`);
                    reject();
                });
        });
    }

    public async estahlishConnectionAsync(): Promise<void> {
        new Promise<void>(async (resolve) => {
            console.info("Establishing connection with the echo server...");

            let numberOfAttempts = EchoService.NUMBER_OF_ATTEMPS;
            let i = 0;
            do {
                await this.getToken();

                if (!this.jwtToken) {
                    console.warn("No authorization token! Connection to the echo server could not be estabished.");
                    console.warn("Retrying in 3 seconds...");
                } else {
                    await this.estahlishConnection();
                    resolve();
                    return;
                }

                await new Promise(r => setTimeout(r, 2000));
                i++;
            } while (i < numberOfAttempts);
            console.warn("Could not reach the echo server.\n\nPlesae refresh the page.");
            resolve();
        });
    }

    public async initSocketConnectionAsync() {
        new Promise<void>(async (resolve) => {
            this.estahlishConnectionAsync();
            resolve();
        });
    }

    public async setMessagesAsSeen(messagesIds: number[], conversationId: number) {

        if(messagesIds.length < 1)
            return;

        this.connection
            .invoke('SetAsSeen', messagesIds, conversationId)
            .catch(
                error => {
                    console.log(`Socket connecion filed: ${error}`);
                }
            );
    }

    public listenForMessagesUpdates() {
        this.connection.on('OnMessageUpdate', 
            (data: IMessageUpdateDto) => {
                this.messagesUpdate.next(data);
            }
        );
    }

    public listenForConversationsUpdates() {
        this.connection.on('OnConversationUpdate', 
            (data: IConversationUpdateDto) => {
                this.conversationsUpdate.next(data);
            }
        );
    }

    public listenForUserUpdates() {
        this.connection.on('OnUserUpdate', 
            (data: IUserUpdateDto) => {

                if(data.type === EUserUpdateType.LeftConversation || data.type === EUserUpdateType.RemovedFromConversation){
                    this.connection
                    .invoke('RemoveFromHub', data.conversationId)
                    .catch(
                        error => {}
                    );
                    this.windowsService.closeWindow(data.conversationId);
                }


                if(data.type === EUserUpdateType.AddedToConversation){
                    this.connection
                    .invoke('AddToHub', data.conversationId)
                    .catch(
                        error => {}
                    );
                    this.windowsService.closeWindow(data.conversationId);
                }


                this.userUpdate.next(data);
            }
        );
    }
}
