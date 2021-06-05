import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';

const ngxBootstrapModules = [BsDropdownModule.forRoot()];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ...ngxBootstrapModules,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
    }),
  ],
  exports: [BsDropdownModule, ToastrModule],
})
export class SharedModule {}
