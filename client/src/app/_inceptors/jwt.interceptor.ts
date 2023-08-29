import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import {AccountService} from "../_services/account.service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    console.log("Interceptor " + this.accountService.currentUser)
    if ( this.accountService.currentUser ) {
      return next.handle(request.clone({
        headers: request.headers.set("Authorization", "Bearer " + this.accountService.currentUser?.token)
      }));
    }
    return next.handle(request);
  }

}
