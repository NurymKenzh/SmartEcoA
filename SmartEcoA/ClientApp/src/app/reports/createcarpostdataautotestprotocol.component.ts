import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ReportService } from './report.service';
import { Report } from './report.model';

import { CarPost } from '../carposts/carpost.model';
import { CarPostService } from '../carposts/carpost.service';

import { CarPostDataAutoTest } from '../carpostdataautotests/carpostdataautotest.model';
import { CarPostDataAutoTestService } from '../carpostdataautotests/carpostdataautotest.service';

@Component({
  templateUrl: 'createcarpostdataautotestprotocol.component.html',
  styleUrls: ['createcarpostdataautotestprotocol.component.css'],
})

export class ReportCreateCarPostDataAutoTestProtocolComponent implements OnInit {
  public reportForm: FormGroup;
  carposts: CarPost[];
  CarPostDate = new FormControl(new Date());
  CarPostId = new FormControl('');
  carpostdataautotests: CarPostDataAutoTest[];

  constructor(private router: Router,
    private service: ReportService,
    private carPostService: CarPostService,
    private carPostDataAutoTestService: CarPostDataAutoTestService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.CarPostId.setValue(this.carposts[0] ? this.carposts[0].Id : null);
        this.CarPostDataAutoTestsUpdate();
      });
    this.reportForm = new FormGroup({
      CarPostDataAutoTestId: new FormControl('', [Validators.required]),
      SelectedTypeReport: new FormControl('', [Validators.required])
    });
    this.reportForm.controls["SelectedTypeReport"].setValue("false");
  }

  public CarPostDataAutoTestsUpdate() {
    this.carPostDataAutoTestService.get(null, this.CarPostId.value, this.CarPostDate.value)
      .subscribe(res => {
        this.carpostdataautotests = res as CarPostDataAutoTest[];
        this.carpostdataautotests.sort((a, b) => (a.DateTime > b.DateTime) ? 1 : ((b.DateTime > a.DateTime) ? -1 : 0));
        this.reportForm.controls["CarPostDataAutoTestId"].setValue(this.carpostdataautotests[0] ? this.carpostdataautotests[0].Id : null);
      });
  }

  public error(control: string,
    error: string) {
    return this.reportForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/reports/create/carposts');
  }

  public create(reportFormValue) {
    if (this.reportForm.valid) {
      const report: Report = {
        Id: 0,
        ApplicationUser: null,
        Name: null,
        NameEN: 'Report of measurements of harmful emissions in the exhaust gases of a motor vehicle',
        NameRU: 'Протокол измерений вредных выбросов в отработавших газах автотранспортного средства',
        NameKK: 'Автокөліктің пайдаланылған газдарындағы зиянды шығарындыларды өлшеу туралы есеп',
        InputParameters: null,
        InputParametersEN: null,
        InputParametersRU: null,
        InputParametersKK: null,
        Inputs: `CarPostDataAutoTestId=${this.reportForm.controls["CarPostDataAutoTestId"].value}`,
        DateTime: null,
        CarPostStartDate: null,
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
