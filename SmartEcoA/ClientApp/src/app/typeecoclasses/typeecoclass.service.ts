import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class TypeEcoClassService {
  private baseUrl: string;
  private apiUrl = 'api/TypeEcoClasses/';

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

  post(typeEcoClass) {
    return this.http.post(this.baseUrl + this.apiUrl, typeEcoClass);
  }

  put(typeEcoClass) {
    return this.http.put(this.baseUrl + this.apiUrl + typeEcoClass.Id, typeEcoClass);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }
}
