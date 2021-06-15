import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { User } from '../models/users';

interface UserCredential {
  userName: string;
  password: string;
}

interface JwtToken {
  nameid: string;
  unique_name: string;
  role: string | string[];
  nbf: number;
  exp: number;
  iat: number;
}

@Injectable({
  // if set to root, no longer need to add into providers array in modules in newer versions of angular
  providedIn: 'root',
})
export class AccountService {
  private endpoint = '/api/account';

  private currentUserSource = new ReplaySubject<User>(1); // only store one value from the stream when next is triggered
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  setCurrentUser(user: User): void {
    if (user !== null) {
      user.roles = [];
      const roles = this.getDecodedToken(user.token).role;
      Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);
    }

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  register(userCredential: UserCredential): Observable<User> {
    return this.http
      .post<User>(this.endpoint + '/register', userCredential)
      .pipe(
        tap((user: User) => {
          if (user) {
            this.setCurrentUser(user);
          }
        })
      );
  }

  login(userCredential: UserCredential): Observable<User> {
    return this.http.post<User>(this.endpoint + '/login', userCredential).pipe(
      tap((user: User) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  getDecodedToken(token: string): JwtToken {
    // atob is a method that allows us to decrypt the parts of a JWT token that does not require a signature
    // [1] refers to the payload property inside a JWT token
    return JSON.parse(atob(token.split('.')[1]));
  }
}
