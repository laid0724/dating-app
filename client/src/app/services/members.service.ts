import {
  HttpClient,
  HttpHeaders,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { take, tap } from 'rxjs/operators';
import { getPaginatedResult, getPaginationHeaders } from '../helpers';
import { Member } from '../models/member';
import { PaginatedResult } from '../models/pagination';
import { UserParams } from '../models/userParams';
import { User } from '../models/users';
import { AccountService } from './account.service';

// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token,
//   }),
// };

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private endpoint = '/api/users';

  // if we cache our results, we wont sent unnecessary http requests
  // and the loading screen wont be triggered
  // members: Member[] = [];
  membersCache = new Map();
  user: User;
  _userParams: UserParams;

  get userParams(): UserParams {
    return this._userParams;
  }

  set userParams(params: UserParams) {
    this._userParams = params;
  }

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    accountService.currentUser$
      // do not use take(1) here because it will not work when user logout and login as another, as observable is complete.
      // .pipe(take(1))
      .subscribe((user: User) => {
        if (user === null) {
          // clear cache when user logs out
          this.membersCache.clear();
        }
        this.user = user;
        this.userParams = new UserParams(user);
      });
  }

  resetUserParams(): UserParams {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams): Observable<PaginatedResult<Member[]>> {
    // new caching mechanism using map class:
    const key = Object.values(userParams).join('-');
    const cachedResultsByKey = this.membersCache.get(key);

    if (cachedResultsByKey) {
      return of(cachedResultsByKey);
    }
    // caching end

    const { pageNumber, pageSize, minAge, maxAge, gender, orderBy } =
      userParams;
    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('minAge', minAge.toString());
    params = params.append('maxAge', maxAge.toString());
    params = params.append('gender', gender);
    params = params.append('orderBy', orderBy);

    return getPaginatedResult<Member[]>(this.http, this.endpoint, params).pipe(
      tap((response: PaginatedResult<Member[]>) => {
        this.membersCache.set(key, response);
      })
    );

    // ORIGINAL CACHING
    // a very rudimentary form of caching
    // doesn't work when entity is updated while you are using the app,
    // unless user refreshes page
    // or if results are paged/filtered.
    // if (this.members.length > 0) {
    //   return of(this.members);
    // }

    // return this.http.get<Member[]>(
    //   this.endpoint,
    //   // , httpOptions
    // ).pipe(tap((members) => (this.members = members)));
  }

  getMember(username: string): Observable<Member> {
    // new caching mechanism using map class:
    const cachedMember = [...this.membersCache.values()]
      .reduce(
        // (payload: Member[], paginatedResult: PaginatedResult<Member[]>) => {
        //   const cachedPaginatedResult: Member[] = paginatedResult.result;

        //   if (payload.length < 1) {
        //     payload = [...cachedPaginatedResult];
        //   }

        //   cachedPaginatedResult.forEach((member: Member) => {
        //     if (!payload.includes(member)) {
        //       payload = [...payload, member];
        //     }
        //   });

        //   return payload;
        // },

        // this is more efficient even though there are duplicating results:
        // .find will exit once it gets the first matching result
        (payload: Member[], paginatedResult: PaginatedResult<Member[]>) =>
          payload.concat(paginatedResult.result),
        []
      )
      .find(
        (m: Member) =>
          m.userName.toLowerCase().trim() === username.toLowerCase().trim()
      );

    if (cachedMember !== undefined) {
      return of(cachedMember);
    }
    // caching end

    return this.http.get<Member>(
      this.endpoint + `/${username}`
      // , httpOptions
    );

    // ORIGINAL CACHING
    // a very rudimentary form of caching
    // doesn't work when entity is updated while you are using the app,
    // unless user refreshes page
    // or if results are paged/filtered
    // const member = this.members.find(
    //   (m: Member) =>
    //     m.userName.toLowerCase().trim() === username.toLowerCase().trim()
    // );

    // if (member !== undefined) {
    //   return of(member);
    // }
  }

  updateMember(member: Member): Observable<null> {
    return this.http.put<null>(this.endpoint, member);

    // cache update no longer needed because API filters out current user

    // ORIGINAL CACHING:
    // .pipe(
    //   // update cached members as well so we dont need to fetch data again
    //   tap(() => {
    //     if (this.members.length > 0) {
    //       const memberToBeUpdated = this.members.find(
    //         (m: Member) =>
    //           m.userName.toLowerCase().trim() ===
    //           member.userName.toLowerCase().trim()
    //       );

    //       if (memberToBeUpdated != null) {
    //         const memberToBeUpdatedIndex =
    //           this.members.indexOf(memberToBeUpdated);
    //         if (memberToBeUpdatedIndex !== -1) {
    //           this.members[memberToBeUpdatedIndex] = member;
    //         }
    //       }
    //     }
    //   })
    // );
  }

  setMainPhoto(photoId: number): Observable<null> {
    return this.http.put<null>(
      this.endpoint + '/set-main-photo/' + photoId,
      {}
    );
  }

  deletePhoto(photoId: number): Observable<null> {
    return this.http.delete<null>(this.endpoint + '/delete-photo/' + photoId);
  }
}
