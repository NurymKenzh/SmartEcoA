import { Inject } from '@angular/core';
import { HttpClient, HttpParams  } from "@angular/common/http";

export class CarPostService {
  private baseUrl: string;
  private apiUrl = 'api/CarPosts/';

  constructor(private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get(Id?) {
    if (Id) {
      return this.http.get(this.baseUrl + this.apiUrl + Id);
    } else {
      return this.http.get(this.baseUrl + this.apiUrl);
    }
  }

  post(carpost) {
    return this.http.post(this.baseUrl + this.apiUrl, carpost);
  }

  put(carpost) {
    return this.http.put(this.baseUrl + this.apiUrl + carpost.Id, carpost);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }

  public report(startDate?, endDate?, carPostsId?) {
    let body = {
      startDate: `${startDate.getFullYear()}-${startDate.getMonth() + 1}-${startDate.getDate()}`,
      endDate: `${endDate.getFullYear()}-${endDate.getMonth() + 1}-${endDate.getDate()} 23:59:59`,
      carPostsId: carPostsId
    };
    return this.http.post(this.baseUrl + this.apiUrl + 'Report', body);
  } 
}
