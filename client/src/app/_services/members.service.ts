import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Member} from "../_models/member";
import {environment} from "../../environments/environment";
import {map, Observable, of, pipe} from "rxjs";
import {PaginatedResult} from "../_models/pagination";
import {UserParams} from "../_models/userParams";

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl: string = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) {
  }

  private getPaginationHeaders(userParams: UserParams) : HttpParams {
    let params = new HttpParams();

    params = params.set('pageNumber', userParams.pageIndex);
    params = params.set('pageSize', userParams.pageSize);
    params = params.set('minAge', userParams.minAge)
    params = params.set('maxAge', userParams.minAge)
    params = params.set('maxAge', userParams.maxAge)
    params = params.set('gender', userParams.gender)
    params = params.set('orderBy', userParams.orderBy)

    return params;
  }
  public getMembers(userParams: UserParams): Observable<PaginatedResult<Member[]>> {

    const params = this.getPaginationHeaders(userParams)

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params);
  }

  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

    console.log(params)
    return this.http.get<T>(url, {observe: 'response', params: params}).pipe(
      map(response => {
        if ( response.body ) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if ( pagination ) {
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })
    )
  }

  getMember(username: string): Observable<Member> {
    const member = this.members.find(x => x.username === username);
    if ( member ) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(map(
      () => {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member}
      }
    ))
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {})
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId, {})
  }
}
