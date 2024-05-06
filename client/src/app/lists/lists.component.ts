import {Component, OnInit} from '@angular/core';
import {Member} from "../_models/member";
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {map} from "rxjs";
import {Job} from "../_models/job";
import {MembersService} from "../_services/members.service";
import {Pagination} from "../_models/pagination";
import {LikesParams} from "../_models/userParams";

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.scss']
})
export class ListsComponent implements OnInit{
  baseUrl: string = environment.apiUrl;
  members: Member[] | undefined;
  likeParams : LikesParams | undefined;
  pagination: Pagination | undefined;

  constructor(private memberService: MembersService) {
    this.likeParams = new LikesParams();
  }
  ngOnInit(): void {
    // this.loadJobs();
    this.loadLikes();
  }

  loadLikes() {
    if(!this.likeParams) return;
    this.memberService.getLikes(this.likeParams).subscribe({
      next: response => {
        this.members = response.result;
        this.pagination = response.pagination;
      }
    })
  }

  pageChanged(event: any) {
    console.log("pageChanged", event)
    if(!this.likeParams) return;
    this.likeParams.pageIndex = event.pageIndex+1;
    this.likeParams.pageSize = event.pageSize;
    this.loadLikes();
  }

  //
  // loadJobs(){
  //   return this.http.get<Job[]>(this.baseUrl + 'jobs', {}).subscribe({
  //     next: jobs => this.jobs = jobs,
  //     error: error => console.log(error)
  //   });
  // }
}
