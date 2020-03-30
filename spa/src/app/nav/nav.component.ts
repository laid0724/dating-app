import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService, UserDTO } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';

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

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {}

  login(): void {
    this.authService.login(this.model).subscribe(
      success => {
        this.alertify.success('Logged in successfully.');
      },
      error => {
        this.alertify.error(`Login failed:: ${error}`);
      }
    );
  }

  loggedIn(): boolean {
    const token = localStorage.getItem('token');
    return !!token;
  }

  logout(): void {
    localStorage.removeItem('token');
    this.alertify.message('Logged out.');
  }
}
