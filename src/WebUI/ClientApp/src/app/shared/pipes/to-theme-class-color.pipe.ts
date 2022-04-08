import { Pipe, PipeTransform } from '@angular/core';
import { ConversationColor } from 'src/app/web-api-client';

@Pipe({
    name: 'toThemeClassColor'
})
export class ToThemeClassColorPipe implements PipeTransform {

    transform(color: ConversationColor, suffix = ""): string {
        return "bg-" + color + suffix;
    }

}
