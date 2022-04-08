import { WindowsService } from '../../services/windows.service';
import { Component, OnInit } from '@angular/core';

import { EventEmitter, HostListener, Input, Output } from '@angular/core';
import { EWindowState } from '../../dtos/ewindow-state';
import { ICords } from '../../dtos/icords';
import { IRect } from '../../dtos/irect';
import { Helpers } from '../../Constants/Helpers';
import { Constants } from '../../Constants/Constants';
import { IWindowInfo } from '../../dtos/iwindow-info';


export enum ITopControls {
    close,
    maximize,
    minimize
}

@Component({
    selector: 'app-window',
    templateUrl: './window.component.html',
    styleUrls: ['./window.component.scss']
})
export class WindowComponent implements OnInit {


    ITopControls = ITopControls;
    EWindowState = EWindowState;

    @Input() public windowHandler: IWindowInfo;

    @Output() windowStateChanged: EventEmitter<IWindowInfo> =
        new EventEmitter<IWindowInfo>();

    constructor(private windowService: WindowsService) {
    }

    ngOnInit(): void {
        let w = this.windowHandler;

        this.windowService.newWindowOpened(this.windowHandler);

        this.windowService.windowStateChanged.subscribe(windows => {

            if(!windows)
                return;

            let window = windows.find(w => w.id === this.windowHandler.id);

            if (window) {
                
                this.windowHandler = window;
            }
        });
    }


    onTopControlClicked(state: ITopControls) {
        if (state == ITopControls.maximize) {
            this.toggleMaixmize();
        } else if (state == ITopControls.close) {
            this.windowHandler.state = EWindowState.CLOSING;
            this.windowService.onWindowStateChanged(this.windowHandler);
        }
        
        this.windowStateChanged.emit(this.windowHandler);
    }


    onWindowDown(event: any) {
        this.windowHandler.moveToTop = true;
        this.windowService.onWindowStateChanged(this.windowHandler);
    }


    private positionOffset: ICords = {
        x: 0,
        y: 0
    }

    private resizeOffset: IRect = {
        x: 0,
        y: 0,
        width: 0,
        height: 0
    }

    private lastWindowedRect: IRect;

    onMoveWindowDown(event: any) {
        if (this.windowHandler.state === EWindowState.MAXIMIZED) {
            return;
        }

        let { clientX: x, clientY: y } = event;
        let windowBox = event.currentTarget.getBoundingClientRect();

        if (windowBox) {
            x -= windowBox.x;
            y -= windowBox.y
        }

        this.positionOffset = {
            x,
            y,
        }
        this.windowHandler.state = EWindowState.MOVING;
    }

    onMoveWindowUp(event: any) {
        this.windowService.saveOpenedWindows();
    }

    onRezizeDown(event: any) {
        if (this.windowHandler.state === EWindowState.MAXIMIZED) {
            return;
        }

        let { width, height } = this.windowHandler.rect;
        let { clientX: x, clientY: y } = event;

        this.resizeOffset = {
            x,
            y,
            width,
            height
        }

        this.windowHandler.state = EWindowState.RESIZING;
    }

    onRezizeUp(event: any) {
        this.windowService.saveOpenedWindows();
    }

    @HostListener('document:mousemove', ['$event'])
    onMouseMove(event: MouseEvent) {
        if (this.windowHandler.state === EWindowState.MAXIMIZED)
            return;

        let { clientX, clientY } = event;
        let { innerWidth: screenWidth, innerHeight: screenHeight } = window;
        switch (this.windowHandler.state) {
            case EWindowState.MOVING:
                this.moveWindow(clientX, clientY, screenWidth, screenHeight);
                break;
            case EWindowState.RESIZING:
                this.resizeWindow(clientX, clientY, screenWidth, screenHeight);
                break;
            default:
                break;
        }
    }


    moveWindow(clientX, clientY, screenWidth, screenHeight) {
        let { rect } = this.windowHandler;
        let { x: offsetLeft, y: offsetTop } = this.positionOffset;
        let { inRange: rage } = Helpers;

        let newPos = {
            x: clientX - offsetLeft,
            y: clientY - offsetTop
        }

        if (rage(newPos.x, 0, screenWidth - rect.width))
            rect.x = newPos.x;

        if (rage(newPos.y, 0, screenHeight - rect.height))
            rect.y = newPos.y;

        this.windowHandler.rect = { ...rect };
    }

    resizeWindow(clientX, clientY, screenWidth, screenHeight) {
        let { rect } = this.windowHandler;
        let { x: offsetLeft, y: offsetTop, height: oldHeight, width: oldWidth } = this.resizeOffset;
        let { inRange: rage } = Helpers;

        let newSize = {
            x: oldWidth + (clientX - offsetLeft),
            y: oldHeight + (clientY - offsetTop)
        }

        if (rage(newSize.x, Constants.MIN_WINDOW_DIMENSIONS.x, screenWidth))
            rect.width = newSize.x;

        if (rage(newSize.y, Constants.MIN_WINDOW_DIMENSIONS.y, screenHeight))
            rect.height = newSize.y;

        this.windowHandler.rect = { ...rect };
    }

    toggleMaixmize() {
        switch (this.windowHandler.state) {
            case EWindowState.WINDOWED: {
                let { rect } = this.windowHandler;
                let { innerWidth: width, innerHeight: height } = window;

                this.lastWindowedRect = rect;
                this.positionOffset = { ...rect };

                this.windowHandler = {
                    ...this.windowHandler,
                    rect: {
                        x: 0,
                        y: 0,
                        width,
                        height
                    },
                    state: EWindowState.MAXIMIZED
                };
                break;
            }
            case EWindowState.MAXIMIZED: {
                this.windowHandler = {
                    ...this.windowHandler,
                    rect: this.lastWindowedRect,
                    state: EWindowState.WINDOWED
                };
                break;
            };
        }
    }

    @HostListener("document:mouseup")
    onMouseUp() {
        if (this.windowHandler.state === EWindowState.MAXIMIZED){
            return;
        }
        this.windowHandler.state = EWindowState.WINDOWED;
    }

}
