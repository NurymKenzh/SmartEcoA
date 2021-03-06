import { Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";

export class PostService {
  private baseUrl: string;
  private apiUrl = 'api/Posts/';

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

  post(post) {
    return this.http.post(this.baseUrl + this.apiUrl, post);
  }

  put(post) {
    return this.http.put(this.baseUrl + this.apiUrl + post.Id, post);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
