import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/users';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(): void {
    this.adminService
      .getUserWithRoles()
      .subscribe((users) => (this.users = users));
  }
}
