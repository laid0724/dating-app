import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/models/member';
import { Pagination } from 'src/app/models/pagination';
import { UserParams } from 'src/app/models/userParams';
import { User } from 'src/app/models/users';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.scss'],
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  genderList = [
    { value: 'male', display: 'Male' },
    { value: 'female', display: 'Female' },
  ];

  constructor(private membersService: MembersService) {
    this.userParams = this.membersService.userParams;
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(): void {
    this.membersService.userParams = this.userParams;
    this.membersService.getMembers(this.userParams).subscribe((members) => {
      this.members = members.result;
      this.pagination = members.pagination;
    });
  }

  onPageChanged(pageChangeEvent: { page: number; itemsPerPage: number }): void {
    this.userParams.pageNumber = pageChangeEvent.page;
    this.membersService.userParams = this.userParams;
    this.loadMembers();
  }

  resetFilters(): void {
    this.userParams = this.membersService.resetUserParams();
  }
}
