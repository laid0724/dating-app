import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PaginatedResult } from './models/pagination';

export function controlHasError(
  form: FormGroup,
  formControlName: string,
  errors: string[]
): boolean {
  const formControl = form.get(formControlName) as FormControl;
  let errorCount = 0;

  errors.forEach((err) => {
    if (formControl.hasError(err)) {
      errorCount++;
    }
  });

  return formControl.touched && formControl.invalid && errorCount > 0;
}

export function getPaginationHeaders(
  pageNumber: number,
  pageSize: number
): HttpParams {
  let params = new HttpParams();

  params = params.append('pageNumber', pageNumber.toString());
  params = params.append('pageSize', pageSize.toString());

  return params;
}

export function getPaginatedResult<T>(
  http: HttpClient,
  url: string,
  params: HttpParams
): Observable<PaginatedResult<T>> {
  const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
  return http
    .get<T>(url, {
      /*
          Angular's http client by default will return the response's body,
          changing this to 'response' will make httpclient return the whole thing,
          along with the headers
        */
      observe: 'response',
      params,
    })
    .pipe(
      map((response: HttpResponse<T>) => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(
            response.headers.get('Pagination')
          );
        }
        return paginatedResult;
      })
    );
}
