import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ReportService } from './report.service';
import { Report } from './report.model';

import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'createcarpostsprotocol.component.html',
  styleUrls: ['createcarpostsprotocol.component.css'],
})

export class ReportCreateCarPostsProtocolComponent implements OnInit {
  public reportForm: FormGroup;
  carposts: CarPost[];
  CarPostDate = new FormControl(new Date());

  constructor(private router: Router,
    private service: ReportService) { }

  ngOnInit() {
    this.reportForm = new FormGroup({
      SelectedTypeReport: new FormControl('', [Validators.required])
    });
    this.reportForm.controls["SelectedTypeReport"].setValue("false");
  }

  public error(control: string,
    error: string) {
    return this.reportForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/reports/create/carposts');
  }

  public create(reportFormValue) {
    var dateTime = new Date(Date.UTC(this.CarPostDate.value.getFullYear(), this.CarPostDate.value.getMonth(), this.CarPostDate.value.getDate(), 0, 0, 0));
    if (this.reportForm.valid) {
      const report: Report = {
        Id: 0,
        ApplicationUser: null,
        Name: null,
        NameEN: 'Report car posts for the day',
        NameRU: 'Протокол автомобильных постов за день',
        NameKK: 'Күніне автокөлік посттарының хаттамасы',
        InputParameters: null,
        InputParametersEN: null,
        InputParametersRU: null,
        InputParametersKK: null,
        Inputs: null,
        DateTime: null,
        CarPostStartDate: dateTime,
        CarPostEndDate: null,
        FileName: null,
        PDF: this.reportForm.controls["SelectedTypeReport"].value
      }
      this.service.post(report)
        .subscribe(() => {
          this.router.navigateByUrl('/reports');
        },
          (error => {
            console.log(error);
          })
        );
    }
  }
}
