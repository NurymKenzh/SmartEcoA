import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class TesterService {
  private baseUrl: string;
  private apiUrl = 'api/Testers/';

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

  post(tester) {
    return this.http.post(this.baseUrl + this.apiUrl, tester);
  }

  put(tester) {
    return this.http.put(this.baseUrl + this.apiUrl + tester.Id, tester);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
