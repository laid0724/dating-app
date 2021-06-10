import {
  HttpClient,
  HttpHeaders,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Member } from '../models/member';
import { PaginatedResult } from '../models/pagination';

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
  members: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private http: HttpClient) {}

  getMembers(
    page?: number,
    itemsPerPage?: number
  ): Observable<PaginatedResult<Member[]>> {
    // a very rudimentary form of caching
    // doesn't work when entity is updated while you are using the app,
    // unless user refreshes page
    // or if results are paged.
    // if (this.members.length > 0) {
    //   return of(this.members);
    // }

    let params = new HttpParams();

    if (page !== null && itemsPerPage != null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }

    return this.http
      .get<Member[]>(this.endpoint, {
        /*
          Angular's http client by default will return the response's body,
          changing this to 'response' will make httpclient return the whole thing,
          along with the headers
        */
        observe: 'response',
        params,
      })
      .pipe(
        map((response: HttpResponse<Member[]>) => {
          this.paginatedResult.result = response.body;
          if (response.headers.get('Pagination') !== null) {
            this.paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return this.paginatedResult;
        })
      );

    // return this.http.get<Member[]>(
    //   this.endpoint,
    //   // , httpOptions
    // ).pipe(tap((members) => (this.members = members)));
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
