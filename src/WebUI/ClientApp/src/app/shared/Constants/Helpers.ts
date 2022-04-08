class Helpers {

    public static inRange(value: number, min: number, max: number): boolean {
        return ((value - min) * (value - max) <= 0.00);
    }

    public static clamp(value: number, min: number, max: number): number{
        return Math.min(Math.max(value, min), max);
    }

    public static randomInteger(min: number, max: number): number{
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

}

export { Helpers }