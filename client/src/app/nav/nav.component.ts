import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from 'src/app/models/users';
import { controlHasError } from '../helpers';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent implements OnInit {
  form: FormGroup;
  currentUser$: Observable<User>;

  controlHasError: (
    form: FormGroup,
    formControlName: string,
    errors: string[]
  ) => boolean = controlHasError;

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
      password: [null, [Validators.required, Validators.minLength(4)]],
    });
  }

  login(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toastr.error(
        'Please make sure that all required fields are filled out.'
      );
      return;
    }

    this.accountService.login(this.form.value).subscribe((user: User) => {
      !user.roles.includes('Member')
        ? this.router.navigateByUrl('/admin')
        : this.router.navigateByUrl('/members');
    });
  }

  logout(): void {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
