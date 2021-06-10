import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/models/member';
import { Pagination } from 'src/app/models/pagination';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.scss'],
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  pageNumber = 1;
  pageSize = 6;

  constructor(private membersService: MembersService) {}

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(): void {
    this.membersService
      .getMembers(this.pageNumber, this.pageSize)
      .subscribe((members) => {
        this.members = members.result;
        this.pagination = members.pagination;
      });
  }

  onPageChanged(pageChangeEmission: {
    page: number;
    itemsPerPage: number;
  }): void {
    this.pageNumber = pageChangeEmission.page;
    this.loadMembers();
  }

}
