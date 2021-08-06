import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class CarPostDataSmokeMeterService {
  private baseUrl: string;
  private apiUrl = 'api/CarPostDataSmokeMeters/';

  constructor(private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get(Id?, CarPostId?, Date?) {
    if (Id && !CarPostId && !Date) {
      return this.http.get(this.baseUrl + this.apiUrl + Id);
    }
    else if (!Id && CarPostId || Date) {
      let params = new HttpParams()
        .set('CarPostId', CarPostId ? CarPostId : '')
        .set('Date', `${Date.getFullYear()}-${Date.getMonth() + 1}-${Date.getDate()}`);
      return this.http.get(this.baseUrl + this.apiUrl, { params: params });
    }
    else {
      return this.http.get(this.baseUrl + this.apiUrl);
    }
  }

  post(carpostdatasmokemeter) {
    return this.http.post(this.baseUrl + this.apiUrl, carpostdatasmokemeter);
  }

  put(carpostdatasmokemeter) {
    return this.http.put(this.baseUrl + this.apiUrl + carpostdatasmokemeter.Id, carpostdatasmokemeter);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
