import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class PostDataService {
  private baseUrl: string;
  private apiUrl = 'api/PostDatas/';

  constructor(private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get(Id?, date?) {
    if (Id) {
      return this.http.get(this.baseUrl + this.apiUrl + Id);
    } else {
      let params = new HttpParams().set('date', `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`);
      return this.http.get(this.baseUrl + this.apiUrl, { params: params });
    }
  }
}
