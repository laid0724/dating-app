import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from 'src/app/models/users';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent implements OnInit {
  form: FormGroup;
  currentUser$: Observable<User>;

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.form = this.buildForm();
    this.currentUser$ = this.accountService.currentUser$;
  }

  buildForm(): FormGroup {
    return this.fb.group({
      userName: [null, Validators.required],
      password: [null, Validators.required],
    });
  }

  login(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.accountService.login(this.form.value).subscribe(
      (res: User) => {
        this.router.navigateByUrl('/members');
      },
      (err: HttpErrorResponse) => {
        console.error(err);
        this.toastr.error(err.error);
      }
    );
  }

  logout(): void {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
