import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService, UserDTO } from '../services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  model: UserDTO = {
    username: null,
    password: null
  };

  constructor(private authService: AuthService) {}

  ngOnInit(): void {}

  login(): void {
    this.authService.login(this.model)
      .subscribe(
        success => {
          console.log('Logged in successfully.');
        },
        error => {
          console.log('Logged in failed:: ', error);
        }
      );
  }

  loggedIn(): boolean {
    const token = localStorage.getItem('token');
    return !!token;
  }

  logout(): void {
    localStorage.removeItem('token');
    console.log('Logged out.');
  }
}
