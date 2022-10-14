import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ReportService } from './report.service';
import { Report, TypesReport } from './report.model';

import { CarPost } from '../carposts/carpost.model';
import { CarPostService } from '../carposts/carpost.service';
import { MatOption, MatSelect } from '@angular/material';

@Component({
  templateUrl: 'createcarpostdatasmokemeterprotocolperiod.component.html',
  styleUrls: ['createcarpostdatasmokemeterprotocolperiod.component.css'],
})

export class ReportCreateCarPostDataSmokeMeterProtocolPeriodComponent implements OnInit {
  public reportForm: FormGroup;
  carposts: CarPost[];
  typesreport: TypesReport;
  CarPostStartDate = new FormControl(new Date());
  CarPostEndDate = new FormControl(new Date());
  CarPostsId = new FormControl();
  public allCarPosts = [];
  SelectedTypeReport = new FormControl('');
  public typesreportValue = [];

  @ViewChild('allSelected', null) private allSelected: MatOption;
  @ViewChild('multiselect', null) multiselect: MatSelect;

  constructor(private router: Router,
    private service: ReportService,
    private carPostService: CarPostService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.forEach(carPost => this.allCarPosts.push({ 'Id': carPost.Id, 'Name': carPost.Name, 'Selected': false }));
      });
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
    let carPostsId = this.CarPostsId.value.join(';');
    var startDate = new Date(Date.UTC(this.CarPostStartDate.value.getFullYear(), this.CarPostStartDate.value.getMonth(), this.CarPostStartDate.value.getDate(), 0, 0, 0));
    var endDate = new Date(Date.UTC(this.CarPostEndDate.value.getFullYear(), this.CarPostEndDate.value.getMonth(), this.CarPostEndDate.value.getDate(), 23, 59, 59));
    if (this.reportForm.valid) {
      const report: Report = {
        Id: 0,
        ApplicationUser: null,
        Name: null,
        NameEN: 'Report of measurements of harmful emissions in the exhaust gases of a motor vehicle (Diesel) for the period',
        NameRU: 'Протокол измерений вредных выбросов в отработавших газах автотранспортного средства (Дизель) за период',
        NameKK: 'Кезеңдегі автокөліктердің пайдаланылған газдарынан (дизельдік отын) зиянды шығарындыларды өлшеу туралы есеп',
        InputParameters: null,
        InputParametersEN: null,
        InputParametersRU: null,
        InputParametersKK: null,
        Inputs: carPostsId,
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
  tosslePerOne() {
    if (this.allSelected.selected) {
      this.allSelected.deselect();
      return false;
    }
    if (this.reportForm.controls.CarPostsId.value.length === this.carposts.length)
      this.allSelected.select();

  }
  toggleAllSelection() {
    if (this.allSelected.selected) {
      this.multiselect.options.forEach((item: MatOption) => item.select());
    } else {
      this.multiselect.options.forEach((item: MatOption) => item.deselect());
    }
  }
}
