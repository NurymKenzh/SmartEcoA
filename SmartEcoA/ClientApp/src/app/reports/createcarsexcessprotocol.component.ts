import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ReportService } from './report.service';
import { Report, TypesReport } from './report.model';

import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'createcarsexcessprotocol.component.html',
  styleUrls: ['createcarsexcessprotocol.component.css'],
})

export class ReportCreateCarsExcessProtocolComponent implements OnInit {
  public reportForm: FormGroup;
  carposts: CarPost[];
  typesreport: TypesReport;
  CarPostStartDate = new FormControl(new Date());
  CarPostEndDate = new FormControl(new Date());
  SelectedTypeReport = new FormControl('');
  public typesreportValue = [];

  constructor(private router: Router,
    private service: ReportService) { }

  ngOnInit() {
    this.reportForm = new FormGroup({});
    this.typesreportValue = Object.keys(TypesReport).filter(type => isNaN(<any>type) && type !== "values" && type != "Excel");
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
        TypeReport: this.SelectedTypeReport.value as TypesReport
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
