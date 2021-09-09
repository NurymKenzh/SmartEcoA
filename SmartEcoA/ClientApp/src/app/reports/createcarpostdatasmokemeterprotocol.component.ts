import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ReportService } from './report.service';
import { Report } from './report.model';

import { CarPost } from '../carposts/carpost.model';
import { CarPostService } from '../carposts/carpost.service';

import { CarPostDataSmokeMeter } from '../carpostdatasmokemeters/carpostdatasmokemeter.model';
import { CarPostDataSmokeMeterService } from '../carpostdatasmokemeters/carpostdatasmokemeter.service';

@Component({
  templateUrl: 'createcarpostdatasmokemeterprotocol.component.html',
  styleUrls: ['createcarpostdatasmokemeterprotocol.component.css'],
})

export class ReportCreateCarPostDataSmokeMeterProtocolComponent implements OnInit {
  public reportForm: FormGroup;
  carposts: CarPost[];
  CarPostDate = new FormControl(new Date());
  CarPostId = new FormControl('');
  carpostdatasmokemeters: CarPostDataSmokeMeter[];

  constructor(private router: Router,
    private service: ReportService,
    private carPostService: CarPostService,
    private carPostDataSmokeMeterService: CarPostDataSmokeMeterService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.CarPostId.setValue(this.carposts[0] ? this.carposts[0].Id : null);
        this.CarPostDataSmokeMetersUpdate();
      });
    this.reportForm = new FormGroup({
      CarPostDataSmokeMeterId: new FormControl('', [Validators.required]),
      SelectedTypeReport: new FormControl('', [Validators.required])
    });
    this.reportForm.controls["SelectedTypeReport"].setValue("false");
  }

  public CarPostDataSmokeMetersUpdate() {
    this.carPostDataSmokeMeterService.get(null, this.CarPostId.value, this.CarPostDate.value)
      .subscribe(res => {
        this.carpostdatasmokemeters = res as CarPostDataSmokeMeter[];
        this.carpostdatasmokemeters.sort((a, b) => (a.DateTime > b.DateTime) ? 1 : ((b.DateTime > a.DateTime) ? -1 : 0));
        this.reportForm.controls["CarPostDataSmokeMeterId"].setValue(this.carpostdatasmokemeters[0] ? this.carpostdatasmokemeters[0].Id : null);
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
        NameEN: 'Report of measurements of harmful emissions in the exhaust gases of a motor vehicle (Diesel)',
        NameRU: 'Протокол измерений вредных выбросов в отработавших газах автотранспортного средства (Дизель)',
        NameKK: 'Автокөліктің пайдаланылған газдарындағы зиянды шығарындыларды өлшеу туралы есеп (Дизель)',
        InputParameters: null,
        InputParametersEN: null,
        InputParametersRU: null,
        InputParametersKK: null,
        Inputs: `CarPostDataSmokeMeterId=${this.reportForm.controls["CarPostDataSmokeMeterId"].value}`,
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
