import { Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

export class CarModelService {
  private baseUrl: string;
  private apiUrl = 'api/CarModels/';

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

  post(carmodel) {
    return this.http.post(this.baseUrl + this.apiUrl, carmodel);
  }

  put(carmodel) {
    return this.http.put(this.baseUrl + this.apiUrl + carmodel.Id, carmodel);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
