import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable, of } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../services/confirm.service';

@Injectable({
  providedIn: 'root',
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  constructor(private confirmService: ConfirmService) {}
  canDeactivate(component: MemberEditComponent): Observable<boolean> {
    if (component.form.dirty) {
      return this.confirmService.confirm();
    }
    return of(true);
  }
}
