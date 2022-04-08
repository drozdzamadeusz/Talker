import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'userIdsListToArray',
    pure: false
})
export class UserIdsListToArrayPipe implements PipeTransform {

    transform(userIds: any[]): string[] {
        return userIds.map(u => u.userId);
    }

}
