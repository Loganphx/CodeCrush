import {Component, OnInit} from '@angular/core';
import {MembersService} from "../../_services/members.service";
import {Member} from "../../_models/member";
import {Observable, take} from "rxjs";
import {Pagination} from "../../_models/pagination";
import {UserParams} from "../../_models/userParams";
import {AccountService} from "../../_services/account.service";
import {User} from "../../_models/user";

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.scss']
})
export class MembersListComponent implements OnInit {
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  user: User | undefined;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}]
  orderByList = [
    {value: 'age', display: 'Age'},
    {value: 'name', display: 'Name'},
    {value: 'created', display: 'Created'},
    {value: 'lastActive', display: 'Last Active'}
  ]

  constructor(private membersService : MembersService, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    })
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    if(!this.userParams) return;
    this.membersService.getMembers(this.userParams).subscribe({
      next: response => {
        if(response.result && response.pagination) {
          this.members = response.result;
          this.pagination = response.pagination;
        }
      }
    })
  }

  resetFilters() {
    if(this.user) {
      this.userParams = new UserParams(this.user);
      this.loadMembers();
    }
  }
  pageChanged(event: any) {
    console.log("pageChanged", event)
    if(!this.userParams) return;
    this.userParams.pageIndex = event.pageIndex+1;
    this.userParams.pageSize = event.pageSize;
    this.loadMembers();
  }
}
