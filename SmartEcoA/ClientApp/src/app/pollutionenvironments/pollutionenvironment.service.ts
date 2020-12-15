import { Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

export class PollutionEnvironmentService {
  private baseUrl: string;
  private apiUrl = 'api/PollutionEnvironments/';

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

  post(pollutionenvironment) {
    return this.http.post(this.baseUrl + this.apiUrl, pollutionenvironment);
  }

  put(pollutionenvironment) {
    return this.http.put(this.baseUrl + this.apiUrl + pollutionenvironment.Id, pollutionenvironment);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
