import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ToastrModule } from 'ngx-toastr';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';

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
  ],
  exports: [BsDropdownModule, ToastrModule, TabsModule, NgxGalleryModule],
})
export class SharedModule {}
