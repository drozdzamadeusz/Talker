import { AvatarComponent } from './shared/avatar/avatar.component';
import { LoadingSpinnerComponent } from './shared/loading-spinner/loading-spinner.component';
import { NewConversationComponent } from './new-conversation/new-conversation.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { TokenComponent } from './token/token.component';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [

    { path: 'abc', component: AvatarComponent },
    
    { path: 'new-conversation', component: NewConversationComponent, canActivate: [AuthorizeGuard] },
    { path: 'start', component: HomeComponent, canActivate: [AuthorizeGuard] },
    // { path: 'todo', component: TodoComponent, canActivate: [AuthorizeGuard] },
    { path: 'token', component: TokenComponent, canActivate: [AuthorizeGuard] },
    // { path: 'conversation', component: ConversationComponent, canActivate: [AuthorizeGuard] },
    // { path: 'conversation/:id', component: ConversationComponent, canActivate: [AuthorizeGuard] },
    
   
    { path: '', redirectTo: 'start', pathMatch: 'full' },
    { path: '**', redirectTo: 'start', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
    exports: [RouterModule],
})
export class AppRoutingModule { }
