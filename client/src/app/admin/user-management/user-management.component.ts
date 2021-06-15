import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { Role, User } from 'src/app/models/users';
import { AdminService } from 'src/app/services/admin.service';

export interface CheckboxOption {
  name: Role;
  value: Role;
  checked: boolean;
}

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;

  bsModalRef: BsModalRef;

  constructor(
    private adminService: AdminService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(): void {
    this.adminService
      .getUserWithRoles()
      .subscribe((users) => (this.users = users));
  }

  openRolesModal(user: User): void {
    const config = {
      class: 'modal-dialog-center',
      initialState: {
        user,
        roles: this.getRolesArray(user),
      },
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe(
      (emittedValues: CheckboxOption[]) => {
        const rolesToUpdate: Role[] = [
          ...emittedValues
            .filter((el: CheckboxOption) => el.checked)
            .map((el: CheckboxOption) => el.name),
        ];

        if (Array.isArray(rolesToUpdate)) {
          this.adminService
            .updateUserRoles(user.userName, rolesToUpdate)
            .subscribe(() => {
              user.roles = [...rolesToUpdate];
            });
        }
      }
    );
  }

  private getRolesArray(user: User): CheckboxOption[] {
    const roles = [];
    const userRoles = user.roles;

    const availableRoles: CheckboxOption[] = [
      { name: 'Admin', value: 'Admin', checked: false },
      { name: 'Moderator', value: 'Moderator', checked: false },
      { name: 'Member', value: 'Member', checked: false },
    ];

    availableRoles.forEach((role: CheckboxOption) => {
      const userHasRole = userRoles.includes(role.name);
      role.checked = userHasRole;
      roles.push(role);
    });

    return roles;
  }
}
