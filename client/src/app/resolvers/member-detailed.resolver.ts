import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { Member } from '../models/member';
import { MembersService } from '../services/members.service';

@Injectable({ providedIn: 'root' })
export class MemberDetailedResolver implements Resolve<Member> {
  constructor(private membersService: MembersService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    return this.membersService.getMember(route.paramMap.get('username'));
  }
}
