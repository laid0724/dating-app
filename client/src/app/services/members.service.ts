import {
  HttpClient,
  HttpHeaders,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { getPaginatedResult, getPaginationHeaders } from '../helpers';
import { Member } from '../models/member';
import { PaginatedResult } from '../models/pagination';
import { UserParams } from '../models/userParams';

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

  constructor(private http: HttpClient) {}

  getMembers(userParams: UserParams): Observable<PaginatedResult<Member[]>> {
    // a very rudimentary form of caching
    // doesn't work when entity is updated while you are using the app,
    // unless user refreshes page
    // or if results are paged.
    // if (this.members.length > 0) {
    //   return of(this.members);
    // }

    // return this.http.get<Member[]>(
    //   this.endpoint,
    //   // , httpOptions
    // ).pipe(tap((members) => (this.members = members)));

    const { pageNumber, pageSize, minAge, maxAge, gender } = userParams;
    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('minAge', minAge.toString());
    params = params.append('maxAge', maxAge.toString());
    params = params.append('gender', gender);

    return getPaginatedResult<Member[]>(this.http, this.endpoint, params);
  }

  getMember(username: string): Observable<Member> {
    // a very rudimentary form of caching
    // doesn't work when entity is updated while you are using the app,
    // unless user refreshes page
    // or if results are paged
    // const member = this.members.find(
    //   (m: Member) =>
    //     m.userName.toLowerCase().trim() === username.toLowerCase().trim()
    // );

    // if (member != null) {
    //   return of(member);
    // }

    return this.http.get<Member>(
      this.endpoint + `/${username}`
      // , httpOptions
    );
  }

  updateMember(member: Member): Observable<null> {
    return this.http.put<null>(this.endpoint, member);
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
