import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Member} from "../_models/member";
import {environment} from "../../environments/environment";
import {map, Observable, of, pipe} from "rxjs";
import {PaginatedResult} from "../_models/pagination";
import {LikesParams, UserParams} from "../_models/userParams";
import {getPaginatedResult, getPaginationHeaders} from "./paginationHelper";

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl: string = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();

  constructor(private http: HttpClient) {
  }

  public getMembers(userParams: UserParams): Observable<PaginatedResult<Member[]>> {
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if(response) return of(response);

    let params = getPaginationHeaders(userParams.pageIndex, userParams.pageSize)

    params = params.set('minAge', userParams.minAge)
    params = params.set('maxAge', userParams.maxAge)
    params = params.set('gender', userParams.gender)
    params = params.set('orderBy', userParams.orderBy)

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
  }


  getMember(username: string): Observable<Member> {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username === username);

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

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(likeParams: LikesParams) {
    let params = getPaginationHeaders(likeParams.pageIndex, likeParams.pageSize);

    params = params.append('predicate', likeParams.predicate);

    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http);
  }
}
