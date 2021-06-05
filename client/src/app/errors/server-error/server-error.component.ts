import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.scss'],
})
export class ServerErrorComponent {
  isProductionEnv: boolean = environment.production;
  error: any;

  constructor(private router: Router) {
    /*
      IMPORTANT: the state of the router can only be accessed INSIDE the constructor, not in OnInit!

      Note: refreshing this page will make the error disappear because the state of the router is derived
            via the previous router action.
    */

    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.error;
  }
}
