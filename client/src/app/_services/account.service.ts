import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, map} from "rxjs";
import {User} from "../_models/user";
import {PresenceService} from "./presence.service";

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
  constructor(private http : HttpClient, private presenceService : PresenceService) { }

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
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    localStorage.setItem("user", JSON.stringify(user));
    this.currentUserSource.next(user)

    this.presenceService.createHubConnection(user);
  }

  logout()
  {
    localStorage.removeItem('user')
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();
  }

  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]))
  }
}
