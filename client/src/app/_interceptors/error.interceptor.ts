import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor, HttpErrorResponse
} from '@angular/common/http';
import {catchError, Observable, of} from 'rxjs';
import {ToastrService} from "ngx-toastr";
import {NavigationExtras, Router} from "@angular/router";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr : ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error : HttpErrorResponse) => {
        return this.errorFunc(error);
      })
    )
  }

  errorFunc(error:HttpErrorResponse) : Observable<HttpEvent<unknown>>
  {
    if(error) {
      switch(error.status) {
        case 400:
          if(error.error.errors)
          {
            const modelStateErrors = [];
            for (const key in error.error.errors)
            {
              if(error.error.errors[key]) modelStateErrors.push(error.error.errors[key])
            }
            throw modelStateErrors.flat();
          }
          else
          {
            this.toastr.error(error.error, error.status.toString())
          }
          break;
        case 404:
          this.toastr.error('Not Found', error.error.message.toString())
          //this.router.navigateByUrl('/not-found')
          break;
        case 500:
          console.log("500", error.error)
          const navigationExtras: NavigationExtras = {state: {error: error.error}};
          this.router.navigateByUrl('/server-error', navigationExtras);
          break;
      }
    }
    throw error;
  }
}
