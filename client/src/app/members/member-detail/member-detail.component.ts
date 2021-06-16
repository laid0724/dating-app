import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Data, ParamMap } from '@angular/router';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Like } from 'src/app/models/like';
import { Member } from 'src/app/models/member';
import { Message } from 'src/app/models/message';
import { LikesService } from 'src/app/services/likes.service';
import { MessageService } from 'src/app/services/message.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.scss'],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  activeTab: TabDirective;

  member: Member;
  messages: Message[] = [];
  likes: Like[] = [];
  onlineUsers: string[] = [];

  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  destroyer$ = new Subject<boolean>();

  get isLikedByUser(): boolean {
    return !!this.likes?.find((like) => like.userName === this.member.userName);
  }

  get isOnline(): boolean {
    return this.onlineUsers.includes(this.member.userName);
  }

  constructor(
    public presence: PresenceService,
    private messageService: MessageService,
    private likesService: LikesService,
    private toastr: ToastrService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    /*
      we are getting the member's data from a resolver instead of using the service,
      so that the data is already available before the component is constructed,
      no need to wait for ngif in the template
    */
    this.route.data.subscribe((data: Data) => {
      this.member = data.member;
      this.galleryImages = this.getImages();
    });

    this.route.queryParamMap.subscribe((params: ParamMap) => {
      params.has('tab')
        ? // tslint:disable-next-line: radix
          this.selectTab(parseInt(params.get('tab')))
        : this.selectTab(0);
    });

    this.loadLikes();

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      },
    ];

    this.presence.onlineUsers$
      .pipe(takeUntil(this.destroyer$))
      .subscribe((onlineUsers: string[]) => (this.onlineUsers = onlineUsers));
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url,
      });
    }
    return imageUrls;
  }

  loadMessages(): void {
    this.messageService
      .getMessageThread(this.member.userName)
      .subscribe((messages) => {
        this.messages = messages;
      });
  }

  loadLikes(): void {
    this.likesService
      .getAllLikes('liked')
      .subscribe((likes) => (this.likes = likes));
  }

  addLike(member: Member): void {
    this.likesService.addLike(member.userName).subscribe(() => {
      this.toastr.success(`You have liked ${member.knownAs}`);
      this.loadLikes();
    });
  }

  unlike(member: Member): void {
    this.likesService.unlike(member.userName).subscribe(() => {
      this.toastr.warning(`You have unliked ${member.knownAs}`);
      this.loadLikes();
    });
  }

  selectTab(tabId: number): void {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective): void {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.loadMessages();
    }
  }

  ngOnDestroy(): void {
    this.destroyer$.next(true);
    this.destroyer$.unsubscribe();
  }
}
