import { ICords } from "../dtos/icords";
import { IRect } from "../dtos/irect";


class Constants{
    public static MIN_WINDOW_DIMENSIONS: ICords = {
        y: 20 * 16,
        x: 15 * 16
    }

    public static DEFAULT_WINDOW_DIMENSIONS: IRect = {
        x: 0,
        y: 0,
        width: 30 * 16,
        height: 40 * 16,
    }

    public static SCREEN_MARGINS: IRect = {
        x: 0,
        y: 0,
        width: 0,
        height: 1 * 16,
    }
}

export { Constants};