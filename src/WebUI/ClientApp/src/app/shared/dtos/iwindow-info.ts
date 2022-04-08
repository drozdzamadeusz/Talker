import { EWindowState } from "./ewindow-state";
import { IRect } from "./irect";
import { EWindowType } from "./ewindow-type";

export interface IWindowInfo {
    id?: number | undefined,
    type: EWindowType,
    state: EWindowState,
    rect: IRect,
    order: number,
    moveToTop: boolean,
    content?: string | ""
}
