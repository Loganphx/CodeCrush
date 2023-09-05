import {Component, OnInit} from '@angular/core';
import {MembersService} from "../../_services/members.service";
import {Member} from "../../_models/Member";
import {Observable} from "rxjs";

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.scss']
})
export class MembersListComponent implements OnInit {
  members$: Observable<Member[]> | undefined;
  constructor(private membersService : MembersService) {

  }

  ngOnInit(): void {
    this.members$ = this.membersService.getMembers();
  }

}
