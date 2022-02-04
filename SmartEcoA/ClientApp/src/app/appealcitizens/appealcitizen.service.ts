import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class AppealCitizenService {
  private baseUrl: string;
  private apiUrl = 'api/AppealCitizens/';

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

  post(appealCitizen) {
    return this.http.post(this.baseUrl + this.apiUrl, appealCitizen);
  }

  public postQuestion(question) {
    return this.http.post(this.baseUrl + this.apiUrl + 'PostQuestion', question);
  }

  public postAnswer(answer) {
    return this.http.post(this.baseUrl + this.apiUrl + 'PostAnswer', answer);
  }

  //put(report) {
  //  return this.http.put(this.baseUrl + this.apiUrl + report.Id, report);
  //}

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }

  public deleteQuestion(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + 'DeleteQuestion/' + Id);
  }

  public deleteAnswer(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + 'DeleteAnswer/' + Id);
  }
}
