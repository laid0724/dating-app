import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ToastrModule } from 'ngx-toastr';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';
import { TimeagoModule } from 'ngx-timeago';

const ngxBootstrapModules = [
  BsDropdownModule.forRoot(),
  TabsModule.forRoot(),
  BsDatepickerModule.forRoot(),
  PaginationModule.forRoot(),
  ButtonsModule.forRoot(),
  ModalModule.forRoot(),
];

const exportedNgxBootstrapModules = [
  BsDropdownModule,
  TabsModule,
  BsDatepickerModule,
  PaginationModule,
  ButtonsModule,
  ModalModule,
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ...ngxBootstrapModules,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
    }),
    NgxGalleryModule, // see: https://github.com/kolkov/ngx-gallery
    NgxSpinnerModule, // see: https://github.com/Napster2210/ngx-spinner
    FileUploadModule, // see: https://valor-software.com/ng2-file-upload/,
    TimeagoModule.forRoot(), // see: https://github.com/ihym/ngx-timeago#readme
  ],
  exports: [
    ...exportedNgxBootstrapModules,
    ToastrModule,
    NgxGalleryModule,
    NgxSpinnerModule,
    FileUploadModule,
    TimeagoModule,
  ],
})
export class SharedModule {}
