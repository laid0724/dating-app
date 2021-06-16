import { Component, OnDestroy, OnInit } from '@angular/core';
import { User } from './models/users';
import { AccountService } from './services/account.service';
import { PresenceService } from './services/presence.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
  constructor(
    private accountService: AccountService,
    private presenceService: PresenceService
  ) {}

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser(): void {
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (user) {
      this.accountService.setCurrentUser(user);
      this.presenceService.createHubConnect(user);
    }
  }

  ngOnDestroy(): void {
    this.presenceService.stopHubConnection();
  }
}
