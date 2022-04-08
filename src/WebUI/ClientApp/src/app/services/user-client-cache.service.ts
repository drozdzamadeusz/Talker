import { IUeserIdDto } from './../web-api-client';

import { mergeMap as _observableMergeMap, catchError as _observableCatch, shareReplay } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf, of } from 'rxjs';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { ApplicationUserDto, UsersClient } from '../web-api-client';


const CACHE_SIZE = 10;

@Injectable({
    providedIn: 'root'
})
export class UserClientCacheService {
    private currentUserCache: Observable<ApplicationUserDto> = undefined;

    private userCache:Map<string, Observable<ApplicationUserDto>> = new Map();
    private userCacheWithoutObservable:Map<string, ApplicationUserDto> = new Map();

    constructor(private userClient: UsersClient) {
    }


    getCurrentUser(): Observable<ApplicationUserDto> {
        if (this.currentUserCache == undefined) {
            this.currentUserCache = this.userClient.getCurrentUser().pipe(
                shareReplay(CACHE_SIZE)
            );
        }

        return this.currentUserCache;
    }

    getUser(userId: string | null | undefined): Observable<ApplicationUserDto> {
        let user = this.userCache.get(userId);
        if (user == undefined) {
            this.userCache.set(
                userId,
                this.userClient.getUser(userId).pipe(
                    shareReplay(CACHE_SIZE)
                )
            );
        }

        return this.userCache.get(userId);
    }

    convertIdsToUsers(usersIds: string[]): Observable<ApplicationUserDto[]>{
        let newObs: Observable<ApplicationUserDto[]> = new Observable((obserber) =>{

            let fromCache: ApplicationUserDto[] = [];
            let userIdsFromCache: string[] = [];

            usersIds.forEach(id =>{
                if(this.userCacheWithoutObservable.has(id)){
                    fromCache.push(this.userCacheWithoutObservable.get(id));
                    userIdsFromCache.push(id);
                }
            });

            usersIds = usersIds.filter(u => !userIdsFromCache.includes(u));
    
            if(usersIds.length > 0){
                let result = this.userClient.getUsers(usersIds).pipe(
                    shareReplay(CACHE_SIZE)
                );
                result.subscribe(
                    result =>{
                        result.forEach(e => {
                            this.userCache.set(
                                e.id,
                                of(e)
                            );
                            this.userCacheWithoutObservable.set(
                                e.id,
                                e
                            );
                        });
                        fromCache = [...fromCache, ...result];
                        obserber.next(result);
                    }
                );
            }else{
                obserber.next(fromCache);
            }


        });

        return newObs;
    }

}
