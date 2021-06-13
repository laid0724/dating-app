import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { startWith, switchMap, takeUntil } from 'rxjs/operators';
import { Like } from 'src/app/models/like';
import { Member } from 'src/app/models/member';
import { Pagination } from 'src/app/models/pagination';
import { UserParams } from 'src/app/models/userParams';
import { User } from 'src/app/models/users';
import { LikesService } from 'src/app/services/likes.service';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.scss'],
})
export class MemberListComponent implements OnInit, OnDestroy {
  members: Member[];
  likes: Like[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  genderList = [
    { value: 'male', display: 'Male' },
    { value: 'female', display: 'Female' },
  ];

  likesRefresher$ = new Subject<any>();
  destroyer$ = new Subject<boolean>();

  constructor(
    private membersService: MembersService,
    private likesService: LikesService
  ) {
    this.userParams = this.membersService.userParams;
  }

  ngOnInit(): void {
    this.loadMembers();
    this.loadLikes();
  }

  loadMembers(): void {
    this.membersService.userParams = this.userParams;
    this.membersService.getMembers(this.userParams).subscribe((members) => {
      this.members = members.result;
      this.pagination = members.pagination;
    });
  }

  refreshLikes(event: any): void {
    this.likesRefresher$.next(event);
  }

  loadLikes(): void {
    this.likesRefresher$
      .pipe(
        startWith(null),
        switchMap(() => this.likesService.getAllLikes('liked')),
        takeUntil(this.destroyer$)
      )
      .subscribe((likes: Like[]) => (this.likes = likes));
  }

  onPageChanged(pageChangeEvent: { page: number; itemsPerPage: number }): void {
    this.userParams.pageNumber = pageChangeEvent.page;
    this.membersService.userParams = this.userParams;
    this.loadMembers();
  }

  resetFilters(): void {
    this.userParams = this.membersService.resetUserParams();
  }

  ngOnDestroy(): void {
    this.destroyer$.next(true);
    this.destroyer$.unsubscribe();
  }
}
