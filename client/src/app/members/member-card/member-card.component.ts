import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Like } from 'src/app/models/like';
import { Member } from 'src/app/models/member';
import { LikesService } from 'src/app/services/likes.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.scss'],
})
export class MemberCardComponent {
  @Input() member: Member;
  @Input() likes: Like[];
  @Output() refreshLikes = new EventEmitter<null>();

  get isLikedByUser(): boolean {
    return !!this.likes?.find((like) => like.userName === this.member.userName);
  }

  constructor(
    private likesService: LikesService,
    private toastr: ToastrService,
    public presence: PresenceService
  ) {}

  updateLikes(): void {
    this.refreshLikes.emit();
  }

  addLike(member: Member): void {
    this.likesService.addLike(member.userName).subscribe(() => {
      this.toastr.success(`You have liked ${member.knownAs}`);
      this.updateLikes();
    });
  }

  unlike(member: Member): void {
    this.likesService.unlike(member.userName).subscribe(() => {
      this.toastr.warning(`You have unliked ${member.knownAs}`);
      this.updateLikes();
    });
  }
}
