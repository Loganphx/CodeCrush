import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import {Observable, take} from 'rxjs';
import {AccountService} from "../_services/account.service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    console.log("Interceptor", this.accountService.currentUser)
    if ( this.accountService.currentUser ) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${this.accountService.currentUser?.token}`
        }
      });
    }
    return next.handle(request);
  }
}
