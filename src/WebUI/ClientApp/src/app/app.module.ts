import { CanActivate } from '@angular/router';
import { EchoService } from './services/echo.service';
import { MaterialModule } from './material.module';
import { SharedModule } from './shared/shared.module';
import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, ErrorHandler, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ModalModule } from 'ngx-bootstrap/modal';
import { AppRoutingModule } from './app-routing.module';
import { TokenComponent } from './token/token.component';
import { HomeComponent } from './home/home.component';
import { NewConversationComponent } from './new-conversation/new-conversation.component';
import { HeaderComponent } from './conversation/header/header.component';
import { MessagesComponent } from './conversation/messages/messages.component';
import { MessageComponent } from './conversation/messages/message/message.component';
import { ConversationWindowComponent } from './conversation/conversation-window/conversation-window.component';
import { WindowsService } from './shared/services/windows.service';
import { ApplicationinsightsAngularpluginErrorService } from '@microsoft/applicationinsights-angularplugin-js';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        TokenComponent,
        NewConversationComponent,
        HeaderComponent,
        MessagesComponent,
        MessageComponent,
        ConversationWindowComponent,
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        MaterialModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        HttpClientModule,
        FormsModule,
        ApiAuthorizationModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        SharedModule,
        ModalModule.forRoot()
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthorizeInterceptor,
            multi: true
        },
        EchoService,{
            provide: APP_INITIALIZER,
            useFactory: (echoSerive: EchoService) => () => echoSerive.initSocketConnectionAsync(),
            deps: [EchoService],
            multi: true,
        },
        WindowsService,{
            provide: APP_INITIALIZER,
            useFactory: (windowService: WindowsService) => () => windowService.initWindowService(),
            deps: [WindowsService],
            multi: true,
        },
        {
            provide: ErrorHandler,
            useClass: ApplicationinsightsAngularpluginErrorService
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
