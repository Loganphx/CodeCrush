import {Component, OnInit} from '@angular/core';
import {Member} from "../_models/member";
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {map} from "rxjs";
import {Job} from "../_models/job";

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.scss']
})
export class ListsComponent implements OnInit{
  baseUrl: string = environment.apiUrl;
  jobs: Job[] = [];
  constructor(private http: HttpClient) {

  }
  ngOnInit(): void {
    this.loadJobs();
  }

  loadJobs(){
    return this.http.get<Job[]>(this.baseUrl + 'jobs', {}).subscribe({
      next: jobs => this.jobs = jobs,
      error: error => console.log(error)
    });
  }
}
