import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  users: any;
  registerMode = false;

  constructor(private toastr: ToastrService) {}

  ngOnInit(): void {}

  registerToggle(): void {
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(toggle: boolean): void {
    this.registerMode = toggle;
  }

  notImplementedMessage(): void {
    this.toastr.info('Oops! This page is not implemented yet!');
  }
}
