import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Like } from '../models/like';

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

  getLikes(predicate: GetLikePredicate): Observable<Like[]> {
    return this.http.get<Like[]>(this.endpoint, { params: { predicate } });
  }

  unlike(username: string): Observable<null> {
    return this.http.delete<null>(this.endpoint + `/${username}`);
  }
}
