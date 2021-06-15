import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Role, User } from '../models/users';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private endpoint = '/api/admin';

  constructor(private http: HttpClient) {}

  getUserWithRoles(): Observable<Partial<User[]>> {
    return this.http.get<Partial<User[]>>(this.endpoint + '/users-with-roles');
  }

  updateUserRoles(username: string, roles: Role[]): Observable<Role[]> {
    return this.http.post<Role[]>(
      this.endpoint + `/edit-roles/${username}?roles=${roles}`,
      {}
    );
  }
}
