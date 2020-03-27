import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpErrorResponse,
  HTTP_INTERCEPTORS
} from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorInterceptor implements HttpInterceptor {
  // * the HttpIntercepter will intercept any HttpEvents and allow you to apply an handler to deal with that HttpEvent

  intercept(
    req: import('@angular/common/http').HttpRequest<any>, // ? req is the HttpEvent
    next: import('@angular/common/http').HttpHandler // ? next is the Handler that does something with the HttpEvent
  ): import('rxjs').Observable<import('@angular/common/http').HttpEvent<any>> {
    return next // handler
      .handle(req) // HttpEvent
      .pipe(
        catchError(error => {
          if (error.status === 401) {
            // Not Authorized
            return throwError(error.statusText);
          }
          if (error instanceof HttpErrorResponse) {
            // every other type of 4xx or 5xx response
            // ! this MUST match the string inside the headers response thrown back by the API! ("Application-Error")
            const applicationError = error.headers.get('Application-Error');
            if (applicationError) {
              // this handles 500 server erros from .NET
              return throwError(applicationError);
            }

            const serverError = error.error; // ? this is how Angular gets server errors from API responses lol.
            let modelStateErrors = ''; // this is for server side validation errors, e.g., pw too short, user exists, etc.

            // ? errors could be nested in an array, loop through them and save as string
            if (serverError.errors && typeof serverError.errors === 'object') {
              Object.keys(serverError.errors).forEach(key => {
                if (serverError.errors[key]) {
                  modelStateErrors +=
                    'Error - ' + key + ':: ' + serverError.errors[key] + '\n';
                }
              });
            }

            return throwError(
              modelStateErrors || serverError || 'Server Error'
            );
          }
        })
      );
  }
}

// this needs to be exported in a specific format to be added to AppModule's providers array:
export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true // ? multi because HTTPInterceptors can have multiple interceptors
};
