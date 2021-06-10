import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { controlHasError } from '../helpers';
import { AccountService } from '../services/account.service';
import { matchValuesValidator } from '../validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter<boolean>();

  form: FormGroup;

  controlHasError: (
    form: FormGroup,
    formControlName: string,
    errors: string[]
  ) => boolean = controlHasError;

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.form = this.buildForm();
  }

  buildForm(): FormGroup {
    const formToBuild = this.fb.group({
      userName: [null, Validators.required],
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
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toastr.error(
        'Please make sure that all required fields are filled out.'
      );
      return;
    }

    const { userName, password } = this.form.value;

    this.accountService.register({ userName, password }).subscribe((res) => {
      this.cancel();
    });
  }

  cancel(): void {
    this.cancelRegister.emit(false);
  }
}
