import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";

export class ReportService {
  private baseUrl: string;
  private apiUrl = 'api/Reports/';

  constructor(private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get(Id?, Date?) {
    if (Id && !Date) {
      return this.http.get(this.baseUrl + this.apiUrl + Id);
    }
    else if (!Id && Date) {
      let params = new HttpParams()
        .set('Date', `${Date.getFullYear()}-${Date.getMonth() + 1}-${Date.getDate()}`);
      return this.http.get(this.baseUrl + this.apiUrl, { params: params });
    }
    else {
      return this.http.get(this.baseUrl + this.apiUrl);
    }
  }

  post(report) {
    return this.http.post(this.baseUrl + this.apiUrl, report);
  }

  put(report) {
    return this.http.put(this.baseUrl + this.apiUrl + report.Id, report);
  }

  delete(Id) {
    return this.http.delete(this.baseUrl + this.apiUrl + Id);
  }

  download(Id, filename: string = null): void {
    this.http.get(this.baseUrl + this.apiUrl + 'Download/' + Id, { responseType: 'blob' as 'json' }).subscribe(
      (response: any) => {
        let dataType = response.type;
        let binaryData = [];
        binaryData.push(response);
        let downloadLink = document.createElement('a');
        downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: dataType }));
        if (filename)
          downloadLink.setAttribute('download', filename);
        document.body.appendChild(downloadLink);
        downloadLink.click();
      }
    )
  }
}
