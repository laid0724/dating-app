import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { startWith, switchMap, takeUntil } from 'rxjs/operators';
import { PhotoForApproval } from 'src/app/models/photo';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.scss'],
})
export class PhotoManagementComponent implements OnInit, OnDestroy {
  photoForModerationRefresher$ = new Subject<null>();
  destroyer$ = new Subject<boolean>();
  photos: PhotoForApproval[] = [];
  loading = true;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadPhotosForModeration();
  }

  refreshPhotos(): void {
    this.loading = true;
    this.photoForModerationRefresher$.next();
  }

  loadPhotosForModeration(): void {
    this.photoForModerationRefresher$
      .pipe(
        startWith(null),
        switchMap(() => this.adminService.getPhotosForModeration()),
        takeUntil(this.destroyer$)
      )
      .subscribe((photos: PhotoForApproval[]) => {
        console.log(photos);
        this.photos = photos;
        this.loading = false;
      });
  }

  approvePhoto(photo: PhotoForApproval): void {
    this.adminService
      .approvePhoto(photo)
      .subscribe((approvedPhoto: PhotoForApproval) => this.refreshPhotos());
  }

  rejectPhoto(photoId: number): void {
    this.adminService
      .rejectPhoto(photoId)
      .subscribe((approvedPhoto: PhotoForApproval) => this.refreshPhotos());
  }

  ngOnDestroy(): void {
    this.destroyer$.next(true);
    this.destroyer$.unsubscribe();
  }
}
