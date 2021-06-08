import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/models/member';
import { User } from 'src/app/models/users';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.scss'],
})
export class MemberEditComponent implements OnInit {
  member: Member;
  user: User;

  form: FormGroup;

  // allows you to access browser events, in this case, binding to the leaving the page event (unload)
  // this triggers the browser API when users leave the page to go to another site
  @HostListener('window:beforeunload', ['$event']) unloadNotification(
    $event: any
  ): void {
    if (this.form.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private accountService: AccountService,
    private membersService: MembersService,
    private fb: FormBuilder,
    private toastr: ToastrService
  ) {
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe((user) => (this.user = user));
  }

  ngOnInit(): void {
    this.form = this.buildForm();
    this.loadMember();
  }

  buildForm(): FormGroup {
    return this.fb.group({
      introduction: [null],
      lookingFor: [null],
      interests: [null],
      city: [null],
      country: [null],
    });
  }

  loadMember(): void {
    this.membersService.getMember(this.user.userName).subscribe((member) => {
      this.member = member;
      this.form.reset(member);
    });
  }

  updateMember(): void {
    this.membersService
      .updateMember({
        ...this.member,
        ...this.form.value
      } as Member)
      .subscribe(() => {
        this.toastr.success('Profile updated successfully.');
        this.loadMember();
      });
  }
}
