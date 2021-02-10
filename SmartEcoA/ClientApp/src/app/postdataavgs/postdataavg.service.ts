import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class PostDataAvgService {
  private baseUrl: string;
  private apiUrl = 'api/PostDataAvgs/';

  constructor(private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get(Id?, Date?, PostId?, MeasuredParameterId?) {
    if (Id) {
      return this.http.get(this.baseUrl + this.apiUrl + Id);
    } else {
      let params = new HttpParams()
        .set('Date', `${Date.getFullYear()}-${Date.getMonth() + 1}-${Date.getDate()}`)
        .set('PostId', PostId ? PostId : '')
        .set('MeasuredParameterId', MeasuredParameterId ? MeasuredParameterId : '');
      return this.http.get(this.baseUrl + this.apiUrl, { params: params });
    }
  }
}
