import { EventEmitter } from '@angular/core';
import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CheckboxOption } from 'src/app/admin/user-management/user-management.component';
import { User } from 'src/app/models/users';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.scss'],
})
export class RolesModalComponent implements OnInit {
  @Input() updateSelectedRoles = new EventEmitter();
  user: User;
  roles: CheckboxOption[];

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {}

  updateRoles(): void {
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }
}
