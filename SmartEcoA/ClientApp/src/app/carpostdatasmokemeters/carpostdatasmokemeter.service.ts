import { Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

export class CarPostDataSmokeMeterService {
  private baseUrl: string;
  private apiUrl = 'api/CarPostDataSmokeMeters/';

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
