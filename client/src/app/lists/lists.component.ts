import { Component, OnDestroy, OnInit } from '@angular/core';
import { combineLatest, of, Subject } from 'rxjs';
import { startWith, switchMap, takeUntil } from 'rxjs/operators';
import { Like } from '../models/like';
import { Member } from '../models/member';
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

  likesRefresher$ = new Subject<GetLikePredicate>();
  destroyer$ = new Subject<boolean>();

  constructor(private likesService: LikesService) {}

  ngOnInit(): void {
    this.loadLikes();
  }

  refreshLikes(predicate: GetLikePredicate): void {
    this.likesRefresher$.next(predicate);
  }

  loadLikes(): void {
    this.likesRefresher$
      .pipe(
        startWith(this.predicate),
        switchMap((predicate: GetLikePredicate) =>
          combineLatest([
            of(predicate),
            this.likesService.getLikes('liked'),
            this.likesService.getLikes('likedBy'),
          ])
        ),
        takeUntil(this.destroyer$)
      )
      .subscribe(
        ([predicate, likes, likedBy]: [GetLikePredicate, Like[], Like[]]) => {
          if (predicate === 'liked') {
            this.members = likes as Partial<Member[]>;
          }
          if (predicate === 'likedBy') {
            this.members = likedBy as Partial<Member[]>;
          }
          this.likes = likes;
        }
      );
  }

  ngOnDestroy(): void {
    this.destroyer$.next(true);
    this.destroyer$.unsubscribe();
  }
}
