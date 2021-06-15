import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../models/users';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private endpoint = '/api/admin';

  constructor(private http: HttpClient) {}

  getUserWithRoles(): Observable<Partial<User[]>> {
    return this.http.get<Partial<User[]>>(this.endpoint + '/users-with-roles');
  }
}
