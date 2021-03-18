import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class CarPostAnalyticService {
  private baseUrl: string;
  private apiUrl = 'api/CarPostAnalytics/';

  constructor(private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get(Id?, CarPostId?, Date?) {
    if (Id) {
      return this.http.get(this.baseUrl + this.apiUrl + Id);
    } else {
      if (CarPostId && Date) {
        let params = new HttpParams()
          .set('carpostid', CarPostId)
          .set('date', Date);
        return this.http.get(this.baseUrl + this.apiUrl, { params: params });
      }
      else {
        return this.http.get(this.baseUrl + this.apiUrl);
      }
    }
  }

  post(carpostanalytic) {
    return this.http.post(this.baseUrl + this.apiUrl, carpostanalytic);
  }

  put(carpostanalytic) {
    return this.http.put(this.baseUrl + this.apiUrl + carpostanalytic.Id, carpostanalytic);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
