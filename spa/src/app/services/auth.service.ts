import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

export interface UserDTO {
  username: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl: string = environment.apiBaseUrl;
  controllerEndpoint = 'auth/';

  constructor(private http: HttpClient) {}

  login(model: UserDTO): Observable<void> {
    return this.http
      .post(this.baseUrl + this.controllerEndpoint + 'login', model)
      .pipe(
        map((response: any) => {
          const user = response;
          if (user) {
            localStorage.setItem('token', user.token);
          }
        })
      );
  }

  register(newUser: UserDTO): Observable<any> {
    return this.http
      .post(this.baseUrl + this.controllerEndpoint + 'register', newUser);
  }
}
