import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { AngularPlugin } from '@microsoft/applicationinsights-angularplugin-js';
import { Observable } from 'rxjs';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { EWindowType } from './shared/dtos/ewindow-type';
import { IWindowInfo } from './shared/dtos/iwindow-info';
import { WindowsService } from './shared/services/windows.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
    title = 'app';

    EWindowType = EWindowType;
    public openedWindows: IWindowInfo[] = [];
    isAuthenticated: Observable<boolean>;

    constructor(private router: Router, private authorizeService: AuthorizeService,
        private windowsService: WindowsService) {
        this.addAppInsights();
    }

    private addAppInsights() {
        var angularPlugin = new AngularPlugin();
        const appInsights = new ApplicationInsights({
            config: {
                instrumentationKey: '6dc03dc3-2b67-4c99-906c-fd3951c19eb1',
                extensions: [angularPlugin],
                extensionConfig: {
                    [angularPlugin.identifier]: { router: this.router }
                }
            }
        });
        appInsights.loadAppInsights();
    }

    ngOnInit(): void {
        this.isAuthenticated = this.authorizeService.isAuthenticated();
        this.windowsService.windowsListChanged.subscribe(
            windows => {
                if (!windows)
                    return;

                this.openedWindows = windows.filter(w => w.id != 0);
            }
        );
    }

}
