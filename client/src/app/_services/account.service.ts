import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, map} from "rxjs";
import {User} from "../_models/user";

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = "https://localhost:5001/api/"

  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  get currentUser(): User | null {
    return this.currentUserSource.getValue();
  }
  users: any;
  constructor(private http : HttpClient) { }

  login(model: any)
  {
    return this.http.post<User>(this.baseUrl + "account/login", model).pipe(
      map((response: any) => {
        const user = response;
        if(user) {
          this.setCurrentUser(user)
        }
      }))
  }

  register(model: any)
  {
    return this.http.post<User>(this.baseUrl + "account/register", model).pipe(
      map(user => {
        if(user) {
          this.setCurrentUser(user)
        }
        //return user;
      })
    )
  }

  setCurrentUser(user: User)
  {
    localStorage.setItem("user", JSON.stringify(user));
    this.currentUserSource.next(user)
  }

  logout()
  {
    localStorage.removeItem('user')
    this.currentUserSource.next(null);
  }
}
