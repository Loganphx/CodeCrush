import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {Role, User} from "../_models/user";
import {RoleParams, UserParams} from "../_models/userParams";
import {getPaginatedResult, getPaginationHeaders} from "./paginationHelper";
import {Observable} from "rxjs";
import {PaginatedResult} from "../_models/pagination";
import {Photo, PhotoForApproval} from "../_models/photo";

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl

  constructor(private http: HttpClient) {
  }


  getUserWithRoles(roleParams: RoleParams): Observable<PaginatedResult<Role[]>> {
    // return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
    let params = getPaginationHeaders(roleParams.pageIndex, roleParams.pageSize)

    params = params.set('orderBy', roleParams.orderBy)

    return getPaginatedResult(this.baseUrl + 'admin/users-with-roles', params, this.http);
  }

  getAvailableRoles() {
    return this.http.get<string[]>(this.baseUrl + 'admin/available-roles')
  }
  updateUserRoles(username: string, roles: string) {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {})
  }

  getUnapprovedPhotos(): Observable<PhotoForApproval[]> {
    return this.http.get<PhotoForApproval[]>(this.baseUrl + 'admin/unapproved-photos')
  }

  approvePhoto(photoId: number) {
    return this.http.post<PhotoForApproval[]>(this.baseUrl + 'admin/approve-photo/' + photoId, {});
  }
  rejectPhoto(photoId: number) {
    return this.http.post<PhotoForApproval[]>(this.baseUrl + 'admin/reject-photo/' + photoId, {});
  }
}
