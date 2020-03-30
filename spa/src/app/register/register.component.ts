import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { UserDTO, AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister: EventEmitter<boolean> = new EventEmitter();

  model: UserDTO = {
    username: null,
    password: null
  };

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {}

  register() {
    this.authService.register(this.model).subscribe(
      success => this.alertify.success('Registration successful.'),
      error => this.alertify.error(error)
    );
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
