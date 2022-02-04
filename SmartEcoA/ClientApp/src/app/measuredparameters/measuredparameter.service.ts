import { Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

export class MeasuredParameterService {
  private baseUrl: string;
  private apiUrl = 'api/MeasuredParameters/';

  constructor(private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get(Id?) {
    if (Id) {
      console.log(Id);
      console.log(this.baseUrl + this.apiUrl + Id);
      return this.http.get(this.baseUrl + this.apiUrl + Id);
    } else {
      return this.http.get(this.baseUrl + this.apiUrl);
    }
  }

  post(measuredparameter) {
    return this.http.post(this.baseUrl + this.apiUrl, measuredparameter);
  }

  put(measuredparameter) {
    return this.http.put(this.baseUrl + this.apiUrl + measuredparameter.Id, measuredparameter);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
