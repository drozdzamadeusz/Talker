import { Pipe, PipeTransform } from '@angular/core';
import { ApplicationUserDto } from 'src/app/web-api-client';

@Pipe({
  name: 'userIdUserDisplayName',
})
export class UserIdUserDisplayNamePipe implements PipeTransform {

    transform(users: ApplicationUserDto[], userId: string, currentUserId: string = null): string{
        try{
            let {firstName: output, lastName} = users.find(u => u.id === userId);

            if(userId === currentUserId){
                output = "you";
            }

            return `${output}`;
        }catch(error){
            return "";
        }
    }
}
