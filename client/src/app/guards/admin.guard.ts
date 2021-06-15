import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../models/users';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root',
})
export class AdminGuard implements CanActivate {
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map((user: User) => {
        const isAdminOrModerator =
          user.roles.includes('Admin') || user.roles.includes('Moderator');

        if (!isAdminOrModerator) {
          this.toastr.error('You cannot enter this area');
        }

        return isAdminOrModerator;
      })
    );
  }
}
