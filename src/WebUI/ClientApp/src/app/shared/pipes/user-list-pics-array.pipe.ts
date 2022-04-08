import { Pipe, PipeTransform } from '@angular/core';
import { ApplicationUserDto } from 'src/app/web-api-client';

@Pipe({
    name: 'userListPicsArray',
    pure: false
})
export class UserListPicsArrayPipe implements PipeTransform {

    transform(users: ApplicationUserDto[], toExclude: string): string[] {
        return users
            .filter(u => u.id !== toExclude)
            .map(u => u.pictureUrl);
    }

}
