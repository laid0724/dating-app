import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../services/account.service';
import { matchValuesValidator } from '../validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter<boolean>();

  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.registerForm = this.initRegisterForm();

    // 18+ only
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initRegisterForm(): FormGroup {
    const formToBuild = this.fb.group({
      gender: ['male'],
      userName: [null, Validators.required],
      knownAs: [null, [Validators.required]],
      dateOfBirth: [null, [Validators.required]],
      city: [null, [Validators.required]],
      country: [null, [Validators.required]],
      password: [
        null,
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)],
      ],
      confirmPassword: [
        null,
        [Validators.required, matchValuesValidator('password')],
      ],
    });

    formToBuild.get('password').valueChanges.subscribe((v) => {
      formToBuild.get('confirmPassword').updateValueAndValidity();
    });

    return formToBuild;
  }

  register(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      this.toastr.error(
        'Please make sure that all required fields are filled out.'
      );
      return;
    }

    const { confirmPassword, ...registerFormValue } = this.registerForm.value;

    this.accountService.register(registerFormValue).subscribe((success) => {
      this.router.navigateByUrl('/members');
    }, (errors) => {
      this.validationErrors = errors;
    });
  }

  cancel(): void {
    this.cancelRegister.emit(false);
  }
}
