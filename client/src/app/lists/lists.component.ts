import { Component, OnDestroy, OnInit } from '@angular/core';
import { combineLatest, of, Subject } from 'rxjs';
import { startWith, switchMap, takeUntil } from 'rxjs/operators';
import { Like } from '../models/like';
import { Member } from '../models/member';
import { PaginatedResult, Pagination } from '../models/pagination';
import { GetLikePredicate, LikesService } from '../services/likes.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.scss'],
})
export class ListsComponent implements OnInit, OnDestroy {
  members: Partial<Member[]>;
  likes: Like[];
  predicate: GetLikePredicate = 'liked';

  pageNumber = 1;
  pageSize = 6;
  pagination: Pagination;

  likesRefresher$ = new Subject<GetLikePredicate>();
  destroyer$ = new Subject<boolean>();

  constructor(private likesService: LikesService) {}

  ngOnInit(): void {
    this.loadLikes();
  }

  refreshLikes(predicate: GetLikePredicate, resetPage?: boolean): void {
    if (resetPage) {
      this.pageNumber = 1;
      this.pagination.currentPage = 1;
    }
    this.likesRefresher$.next(predicate);
  }

  loadLikes(): void {
    this.likesRefresher$
      .pipe(
        startWith(this.predicate),
        switchMap((predicate: GetLikePredicate) =>
          combineLatest([
            of(predicate),
            this.likesService.getAllLikes('liked'),
            this.likesService.getLikes('liked', this.pageNumber, this.pageSize),
            this.likesService.getLikes(
              'likedBy',
              this.pageNumber,
              this.pageSize
            ),
          ])
        ),
        takeUntil(this.destroyer$)
      )
      .subscribe(
        ([predicate, allLikes, pagedLikes, pagedLikedBy]: [
          GetLikePredicate,
          Like[],
          PaginatedResult<Like[]>,
          PaginatedResult<Like[]>
        ]) => {
          if (predicate === 'liked') {
            this.members = pagedLikes.result as Partial<Member[]>;
            this.pagination = pagedLikes.pagination;
          }
          if (predicate === 'likedBy') {
            this.members = pagedLikedBy.result as Partial<Member[]>;
            this.pagination = pagedLikedBy.pagination;
          }
          this.likes = allLikes;
        }
      );
  }

  onPageChanged(pageChangeEvent: { page: number; itemsPerPage: number }): void {
    this.pageNumber = pageChangeEvent.page;
    this.refreshLikes(this.predicate);
  }

  ngOnDestroy(): void {
    this.destroyer$.next(true);
    this.destroyer$.unsubscribe();
  }
}
