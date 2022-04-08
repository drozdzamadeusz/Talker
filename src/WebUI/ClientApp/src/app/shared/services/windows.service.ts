import { IWindowInfo } from 'src/app/shared/dtos/iwindow-info';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { EWindowState } from '../dtos/ewindow-state';
import { EWindowType } from '../dtos/ewindow-type';

@Injectable({
    providedIn: 'root'
})
export class WindowsService {

    public openedWindows: IWindowInfo[] = [];
    public windowStateChanged: BehaviorSubject<IWindowInfo[]> = new BehaviorSubject<IWindowInfo[]>(null);
    public windowsListChanged: BehaviorSubject<IWindowInfo[]> = new BehaviorSubject<IWindowInfo[]>(null);

    constructor() {
    }

    initWindowService() {
        let lastOpenedWindows: IWindowInfo[] = JSON.parse(localStorage.getItem('windows.opened'));

        if (!lastOpenedWindows)
            return;

        lastOpenedWindows.forEach(w => {
            if (!this.openedWindows.find(o => o.id === w.id))
                this.openNewWindow({
                    ...w,
                    state: EWindowState.WINDOWED,
                });
        });

        this.windowsListChanged.next(this.openedWindows);
    }

    onWindowStateChanged(window: IWindowInfo) {
        let windows = this.openedWindows;

        if (window.state == EWindowState.CLOSING) {

            windows = windows.filter(i => i.id !== window.id);
            window.state = EWindowState.CLOSED;

        } else if (window.moveToTop) {
            this.moveToTop(window.id);
        }

        this.openedWindows = windows;

        if (window.state == EWindowState.CLOSED) {
            this.saveOpenedWindows();
            this.windowsListChanged.next(this.openedWindows);
        }

        this.windowStateChanged.next(windows);
    }


    newWindowOpened(window: IWindowInfo) {
        if (window.id == 0) window.id = this.openedWindows.length;
        if (window.order == 0) window.order = this.openedWindows.length;

        this.windowStateChanged.next(this.openedWindows);
    }


    openNewWindow(window: IWindowInfo) {
        if (!window.id && window.type === EWindowType.NOTEPAD)
            window.id = this.openedWindows.filter(n => n.type === EWindowType.NOTEPAD).length + 100000000;

        this.openedWindows.push(window);
        this.windowsListChanged.next(this.openedWindows);

        this.saveOpenedWindows();
    }

    moveToTop(windowId: number) {
        let windows = this.openedWindows;
        windows = windows.sort((a, b) => a.order - b.order);


        let oldMaxOrderWindow = windows[windows.length - 1];
        let { order: maxOrder } = oldMaxOrderWindow;

        let window = windows.find(w => w.id === windowId);

        oldMaxOrderWindow.order = window.order;
        window.order = maxOrder;
        window.moveToTop = false;
    }

    closeWindow(windowId: number) {
        let windows = this.openedWindows;
        windows = windows.filter(i => i.id !== windowId);
        this.openedWindows = windows;
        this.windowsListChanged.next(this.openedWindows);

        this.saveOpenedWindows();
    }

    saveOpenedWindows() {
        localStorage.setItem('windows.opened', JSON.stringify(this.openedWindows));
    }

    setContent(windowId: number, content: string) {
        let window = this.openedWindows.find(w => w.id === windowId);
        if (window) {
            window.content = content;
            this.saveOpenedWindows();
        }
    }

    getContent(windowId: number): string {
        return this.openedWindows.find(w => w.id === windowId).content;
    }
}
