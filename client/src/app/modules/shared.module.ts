import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ToastrModule } from 'ngx-toastr';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';

const ngxBootstrapModules = [BsDropdownModule.forRoot(), TabsModule.forRoot()];

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
  ],
  exports: [
    BsDropdownModule,
    ToastrModule,
    TabsModule,
    NgxGalleryModule,
    NgxSpinnerModule,
  ],
})
export class SharedModule {}
