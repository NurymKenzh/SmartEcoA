import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ReportService } from './report.service';
import { Report } from './report.model';

import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'createcarsexcessprotocol.component.html',
  styleUrls: ['createcarsexcessprotocol.component.css'],
})

export class ReportCreateCarsExcessProtocolComponent implements OnInit {
  public reportForm: FormGroup;
  carposts: CarPost[];
  CarPostStartDate = new FormControl(new Date());
  CarPostEndDate = new FormControl(new Date());

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
    var startDate = new Date(Date.UTC(this.CarPostStartDate.value.getFullYear(), this.CarPostStartDate.value.getMonth(), this.CarPostStartDate.value.getDate(), 0, 0, 0));
    var endDate = new Date(Date.UTC(this.CarPostEndDate.value.getFullYear(), this.CarPostEndDate.value.getMonth(), this.CarPostEndDate.value.getDate(), 23, 59, 59));
    if (this.reportForm.valid) {
      const report: Report = {
        Id: 0,
        ApplicationUser: null,
        Name: null,
        NameEN: 'Report on repeated exceedances cars at posts',
        NameRU: 'Протокол о повторных превышениях автомобилей на постах',
        NameKK: 'Өткiзу пункттерiнде көлiк құралдарын қайталап асып кету туралы хаттама',
        InputParameters: null,
        InputParametersEN: null,
        InputParametersRU: null,
        InputParametersKK: null,
        Inputs: null,
        DateTime: null,
        CarPostStartDate: startDate,
        CarPostEndDate: endDate,
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
