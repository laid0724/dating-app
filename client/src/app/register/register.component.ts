import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { controlHasError } from '../helpers';
import { AccountService } from '../services/account.service';

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
    return this.fb.group({
      userName: [null, Validators.required],
      password: [null, [Validators.required, Validators.minLength(4)]],
    });
  }

  register(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toastr.error(
        'Please make sure that all required fields are filled out.'
      );
      return;
    }

    this.accountService.register(this.form.value).subscribe((res) => {
      this.cancel();
    });
  }

  cancel(): void {
    this.cancelRegister.emit(false);
  }
}
