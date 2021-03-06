import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService, UserDTO } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { Router } from "@angular/router";

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
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  login(): void {
    this.authService.login(this.model).subscribe(
      success => {
        this.alertify.success('Logged in successfully.');
      },
      error => {
        this.alertify.error(`Login failed:: ${error}`);
      },
      () => { // ? third params in subsribe (no arguments allowed) is for observable complete
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn(): boolean {
    return this.authService.loggedIn();
  }

  logout(): void {
    localStorage.removeItem('token');
    this.alertify.message('Logged out.');
    this.router.navigate(['/']);
  }
}
