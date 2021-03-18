import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class CarModelAutoTestService {
  private baseUrl: string;
  private apiUrl = 'api/CarModelAutoTests/';

  constructor(private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get(Id?, CarPostId?) {
    if (Id) {
      return this.http.get(this.baseUrl + this.apiUrl + Id);
    } else {
      if (CarPostId) {
        let params = new HttpParams().set('carpostid', CarPostId);
        return this.http.get(this.baseUrl + this.apiUrl, { params: params });
      } else {
        return this.http.get(this.baseUrl + this.apiUrl);
      }
    }
  }

  post(carmodelautotest) {
    return this.http.post(this.baseUrl + this.apiUrl, carmodelautotest);
  }

  put(carmodelautotest) {
    return this.http.put(this.baseUrl + this.apiUrl + carmodelautotest.Id, carmodelautotest);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
