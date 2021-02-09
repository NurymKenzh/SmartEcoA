import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class CarModelSmokeMeterService {
  private baseUrl: string;
  private apiUrl = 'api/CarModelSmokeMeters/';

  constructor(private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get(Id?, CarPostId?) {
    if (Id) {
      return this.http.get(this.baseUrl + this.apiUrl + Id);
    } else {
      let params = new HttpParams().set('carpostid', CarPostId);
      return this.http.get(this.baseUrl + this.apiUrl, { params: params });
    }
  }

  post(carmodelsmokemeter) {
    return this.http.post(this.baseUrl + this.apiUrl, carmodelsmokemeter);
  }

  put(carmodelsmokemeter) {
    return this.http.put(this.baseUrl + this.apiUrl + carmodelsmokemeter.Id, carmodelsmokemeter);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
