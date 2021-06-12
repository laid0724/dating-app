import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

/*
  Provide this in your app module:

  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true,
    },
  ],
*/

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400: // bad requests
              const errors = error.error.errors; // for validation errors, which is stored as array with several dimensions
              if (errors) {
                const modalStateErrors = [];
                for (const key in errors) {
                  if (errors[key]) {
                    modalStateErrors.push(errors[key]);
                  }
                }
                throw modalStateErrors.flat(Infinity);
              } else {
                this.toastr.error(
                  error.error,
                  `${error.status.toString()} ${error.statusText}`
                );
              }
              break;
            case 401: // Unauthorized
              this.toastr.error(
                error.error,
                `${error.status.toString()} ${error.statusText}`
              );
              break;
            case 404: // Not Found
              // NOTE: be careful with this - your APIs cannot throw 404 unless it really is a case of 404.
              this.router.navigateByUrl('/not-found');
              break;
            case 500: // Internal Server Error
              // here we redirect as well but we add on NavigationExtras as a message to be passed to the page
              const navigationExtras: NavigationExtras = {
                state: {
                  error: error.error,
                },
              };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong.');
              console.log('Error Caught::', error);
              break;
          }
          return throwError(error);
        }
      })
    );
  }
}
