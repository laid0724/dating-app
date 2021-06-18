import { EventEmitter } from '@angular/core';
import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { CheckboxOption } from 'src/app/admin/user-management/user-management.component';
import { User } from 'src/app/models/users';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.scss'],
})
export class RolesModalComponent implements OnInit {
  @Input() updateSelectedRoles = new EventEmitter();
  user: User;
  roles: CheckboxOption[];
  currentUser$: Observable<User>;

  get noneSelected(): boolean {
    return this.roles
      .map(({ checked }) => checked)
      .every((checked) => !checked);
  }

  constructor(
    public bsModalRef: BsModalRef,
    private accountService: AccountService
  ) {
    this.currentUser$ = accountService.currentUser$;
  }

  ngOnInit(): void {}

  updateRoles(): void {
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }
}
