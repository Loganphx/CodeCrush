import {HttpClient, HttpParams} from "@angular/common/http";
import {PaginatedResult} from "../_models/pagination";
import {map} from "rxjs";

export function getPaginatedResult<T>(url: string, params: HttpParams, http: HttpClient) {
  const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

  console.log(params)
  return http.get<T>(url, {observe: 'response', params: params}).pipe(
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
export function getPaginationHeaders(pageNumber: number, pageSize: number) : HttpParams
{
  let params = new HttpParams();

  params = params.set('pageNumber', pageNumber);
  params = params.set('pageSize', pageSize);

  return params;

}
