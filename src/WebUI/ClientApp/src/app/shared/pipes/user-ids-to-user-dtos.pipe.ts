import { Pipe, PipeTransform } from '@angular/core';
import { ApplicationUserDto } from 'src/app/web-api-client';

@Pipe({
    name: 'userIdsToUserDtos',
    pure: false
})
export class UserIdsToUserDtosPipe implements PipeTransform {

    transform(userIds: string[], users: ApplicationUserDto[]): ApplicationUserDto[] {
        return users.filter(u => userIds.includes(u.id));
    }

}
