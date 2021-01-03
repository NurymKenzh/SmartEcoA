import { Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

export class ProjectService {
  private baseUrl: string;
  private apiUrl = 'api/Projects/';

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

  post(project) {
    return this.http.post(this.baseUrl + this.apiUrl, project);
  }

  put(project) {
    return this.http.put(this.baseUrl + this.apiUrl + project.Id, project);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
