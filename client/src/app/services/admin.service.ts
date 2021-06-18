import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { PhotoForApproval } from '../models/photo';
import { Role, User } from '../models/users';
import { MembersService } from './members.service';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private endpoint = '/api/admin';
  private moderatePhotoEndpoint = this.endpoint + '/photos-to-moderate';

  constructor(
    private http: HttpClient,
    private membersService: MembersService
  ) {}

  getUserWithRoles(): Observable<Partial<User[]>> {
    return this.http.get<Partial<User[]>>(this.endpoint + '/users-with-roles');
  }

  updateUserRoles(username: string, roles: Role[]): Observable<Role[]> {
    return this.http.post<Role[]>(
      this.endpoint + `/edit-roles/${username}?roles=${roles}`,
      {}
    );
  }

  getPhotosForModeration(): Observable<PhotoForApproval[]> {
    return this.http.get<PhotoForApproval[]>(this.moderatePhotoEndpoint);
  }

  approvePhoto(photoId: number): Observable<PhotoForApproval> {
    return this.http
      .post<PhotoForApproval>(this.moderatePhotoEndpoint + `/${photoId}`, {})
      .pipe(tap(() => this.membersService.resetCache()));
  }

  rejectPhoto(photoId: number): Observable<null> {
    return this.http
      .delete<null>(this.moderatePhotoEndpoint + `/${photoId}`)
      .pipe(tap(() => this.membersService.resetCache()));
  }
}
