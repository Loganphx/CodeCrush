import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, map} from "rxjs";
import {IUser} from "../_models/IUser";

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = "https://localhost:5001/api/"

  private currentUserSource = new BehaviorSubject<IUser | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  get currentUser(): IUser | null {
    return this.currentUserSource.getValue();
  }
  users: any;
  constructor(private http : HttpClient) { }

  login(model: any)
  {
    return this.http.post<IUser>(this.baseUrl + "account/login", model).pipe(
      map((response: any) => {
        const user = response;
        if(user) {
          localStorage.setItem('user', JSON.stringify(user))
          this.currentUserSource.next(user)
        }
      }))
  }

  register(model: any)
  {
    return this.http.post<IUser>(this.baseUrl + "account/register", model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem("user", JSON.stringify(user));
          this.currentUserSource.next(user);
        }
        //return user;
      })
    )
  }

  setCurrentUser(user: IUser)
  {
    this.currentUserSource.next(user)
  }

  logout()
  {
    localStorage.removeItem('user')
    this.currentUserSource.next(null);
  }
}
