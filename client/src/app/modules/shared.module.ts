import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ToastrModule } from 'ngx-toastr';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';

const ngxBootstrapModules = [
  BsDropdownModule.forRoot(),
  TabsModule.forRoot(),
  BsDatepickerModule.forRoot(),
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
    FileUploadModule, // see: https://valor-software.com/ng2-file-upload/
  ],
  exports: [
    BsDropdownModule,
    ToastrModule,
    TabsModule,
    NgxGalleryModule,
    NgxSpinnerModule,
    FileUploadModule,
    BsDatepickerModule,
  ],
})
export class SharedModule {}
