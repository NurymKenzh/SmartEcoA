import { Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

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
}
