import { Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

export class CarPostDataAutoTestService {
  private baseUrl: string;
  private apiUrl = 'api/CarPostDataAutoTests/';

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

  post(carpostdataautotest) {
    return this.http.post(this.baseUrl + this.apiUrl, carpostdataautotest);
  }

  put(carpostdataautotest) {
    return this.http.put(this.baseUrl + this.apiUrl + carpostdataautotest.Id, carpostdataautotest);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
