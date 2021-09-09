import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ReportService } from './report.service';
import { Report } from './report.model';

import { CarPost } from '../carposts/carpost.model';
import { CarPostService } from '../carposts/carpost.service';

@Component({
  templateUrl: 'createcarpostdataautotestlog.component.html',
  styleUrls: ['createcarpostdataautotestlog.component.css'],
})

export class ReportCreateCarPostDataAutoTestLogComponent implements OnInit {
  public reportForm: FormGroup;
  carposts: CarPost[];
  CarPostStartDate = new FormControl(new Date());
  CarPostEndDate = new FormControl(new Date());
  CarPostId = new FormControl('');

  constructor(private router: Router,
    private service: ReportService,
    private carPostService: CarPostService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.CarPostId.setValue(this.carposts[0] ? this.carposts[0].Id : null);
      });
    this.reportForm = new FormGroup({
      CarPostDataAutoTestId: new FormControl('', [Validators.required]),
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
    const report: Report = {
        Id: 0,
        ApplicationUser: null,
        Name: null,
        NameEN: 'Vehicle Emission Test Results Log (Gasoline)',
        NameRU: 'Журнал результатов испытаний выбросов автотранспорта (Бензин)',
        NameKK: 'Автокөлік эмиссиясын тексеру нәтижелерінің журналы (Бензин)',
        InputParameters: null,
        InputParametersEN: null,
        InputParametersRU: null,
        InputParametersKK: null,
        Inputs: `CarPostId=${this.CarPostId.value}`,
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
