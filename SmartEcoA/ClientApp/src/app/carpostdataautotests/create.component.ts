import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarPostDataAutoTestService } from './carpostdataautotest.service';
import { CarPostDataAutoTest } from './carpostdataautotest.model';

import { CarModelAutoTestService } from '../carmodelautotests/carmodelautotest.service';
import { CarModelAutoTest } from '../carmodelautotests/carmodelautotest.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'create.component.html'
})

export class CarPostDataAutoTestCreateComponent implements OnInit, AfterViewInit {
  public carpostdataautotestForm: FormGroup;
  carposts: CarPost[];
  carmodelautotests: CarModelAutoTest[];

  constructor(private router: Router,
    private service: CarPostDataAutoTestService,
    private carModelAutoTestService: CarModelAutoTestService,
    private carPostService: CarPostService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.carpostdataautotestForm.controls["CarPostId"].setValue(this.carposts[0] ? this.carposts[0].Id : null);
        this.onCarPostChange();
      });
    this.carpostdataautotestForm = new FormGroup({
      DateTime: new FormControl(new Date(), [Validators.required]),
      Number: new FormControl('', [Validators.maxLength(10)]),
      DOPOL1: new FormControl(''),
      DOPOL2: new FormControl(''),
      MIN_TAH: new FormControl(''),
      MIN_CO: new FormControl(''),
      MIN_CH: new FormControl(''),
      MIN_CO2: new FormControl(''),
      MIN_O2: new FormControl(''),
      MIN_L: new FormControl(''),
      MAX_TAH: new FormControl(''),
      MAX_CO: new FormControl(''),
      MAX_CH: new FormControl(''),
      MAX_CO2: new FormControl(''),
      MAX_O2: new FormControl(''),
      MAX_L: new FormControl(''),
      ZAV_NOMER: new FormControl(''),
      K_1: new FormControl(''),
      K_2: new FormControl(''),
      K_3: new FormControl(''),
      K_4: new FormControl(''),
      K_SVOB: new FormControl(''),
      K_MAX: new FormControl(''),
      MIN_NO: new FormControl(''),
      MAX_NO: new FormControl(''),
      ATNUM: new FormControl(''),
      MIN_NOx: new FormControl(''),
      MAX_NOx: new FormControl(''),
      Version: new FormControl(''),
      CarPostId: new FormControl('', [Validators.required]),
      CarModelAutoTestId: new FormControl('', [Validators.required]),
    });
  }

  ngAfterViewInit() {
    this.carpostdataautotestForm.controls["DateTime"].setValue(new Date());
  }

  public onCarPostChange() {
    this.carModelAutoTestService.get(null, this.carpostdataautotestForm.controls["CarPostId"].value)
      .subscribe(res => {
        this.carmodelautotests = res as CarModelAutoTest[];
        this.carmodelautotests.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.carpostdataautotestForm.controls["CarModelAutoTestId"].setValue(this.carmodelautotests[0] ? this.carmodelautotests[0].Id : null);
      });
  }

  public error(control: string,
    error: string) {
    return this.carpostdataautotestForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carpostdataautotests');
  }

  public create(carpostdataautotestFormValue) {
    if (this.carpostdataautotestForm.valid) {
      const carpostdataautotest: CarPostDataAutoTest = {
        Id: 0,
        DateTime: carpostdataautotestFormValue.DateTime.toLocaleString(),
        Number: carpostdataautotestFormValue.Number,
        DOPOL1: carpostdataautotestFormValue.DOPOL1,
        DOPOL2: carpostdataautotestFormValue.DOPOL2,
        MIN_TAH: carpostdataautotestFormValue.MIN_TAH,
        MIN_CO: carpostdataautotestFormValue.MIN_CO,
        MIN_CH: carpostdataautotestFormValue.MIN_CH,
        MIN_CO2: carpostdataautotestFormValue.MIN_CO2,
        MIN_O2: carpostdataautotestFormValue.MIN_O2,
        MIN_L: carpostdataautotestFormValue.MIN_L,
        MAX_TAH: carpostdataautotestFormValue.MAX_TAH,
        MAX_CO: carpostdataautotestFormValue.MAX_CO,
        MAX_CH: carpostdataautotestFormValue.MAX_CH,
        MAX_CO2: carpostdataautotestFormValue.MAX_CO2,
        MAX_O2: carpostdataautotestFormValue.MAX_O2,
        MAX_L: carpostdataautotestFormValue.MAX_L,
        ZAV_NOMER: carpostdataautotestFormValue.ZAV_NOMER,
        K_1: carpostdataautotestFormValue.K_1,
        K_2: carpostdataautotestFormValue.K_2,
        K_3: carpostdataautotestFormValue.K_3,
        K_4: carpostdataautotestFormValue.K_4,
        K_SVOB: carpostdataautotestFormValue.K_SVOB,
        K_MAX: carpostdataautotestFormValue.K_MAX,
        MIN_NO: carpostdataautotestFormValue.MIN_NO,
        MAX_NO: carpostdataautotestFormValue.MAX_NO,
        ATNUM: carpostdataautotestFormValue.ATNUM,
        MIN_NOx: carpostdataautotestFormValue.MIN_NOx,
        MAX_NOx: carpostdataautotestFormValue.MAX_NOx,
        Version: carpostdataautotestFormValue.Version,
        CarModelAutoTestId: carpostdataautotestFormValue.CarModelAutoTestId,
        CarModelAutoTest: null,
      }
      this.service.post(carpostdataautotest)
        .subscribe(() => {
          this.router.navigateByUrl('/carpostdataautotests');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
