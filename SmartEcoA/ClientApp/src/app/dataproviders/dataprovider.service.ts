import { Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

export class DataProviderService {
  private baseUrl: string;
  private apiUrl = 'api/DataProviders/';

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

  post(dataprovider) {
    return this.http.post(this.baseUrl + this.apiUrl, dataprovider);
  }

  put(dataprovider) {
    return this.http.put(this.baseUrl + this.apiUrl + dataprovider.Id, dataprovider);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
