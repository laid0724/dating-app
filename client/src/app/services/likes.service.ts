import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { getPaginatedResult, getPaginationHeaders } from '../helpers';
import { Like } from '../models/like';
import { PaginatedResult } from '../models/pagination';

export type GetLikePredicate = 'liked' | 'likedBy';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  private endpoint = '/api/likes';

  constructor(private http: HttpClient) {}

  addLike(username: string): Observable<null> {
    return this.http.post<null>(this.endpoint + `/${username}`, {});
  }

  getAllLikes(predicate: GetLikePredicate): Observable<Like[]> {
    return this.http.get<Like[]>(this.endpoint + '/all', {
      params: { predicate },
    });
  }

  getLikes(
    predicate: GetLikePredicate,
    pageNumber: number,
    pageSize: number
  ): Observable<PaginatedResult<Like[]>> {
    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    return getPaginatedResult(this.http, this.endpoint, params);
  }

  unlike(username: string): Observable<null> {
    return this.http.delete<null>(this.endpoint + `/${username}`);
  }
}
