import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarPostDataAutoTestService } from './carpostdataautotest.service';
import { CarPostDataAutoTest } from './carpostdataautotest.model';

import { CarModelAutoTestService } from '../carmodelautotests/carmodelautotest.service';
import { CarModelAutoTest } from '../carmodelautotests/carmodelautotest.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class CarPostDataAutoTestEditComponent implements OnInit {
  public carpostdataautotestForm: FormGroup;
  carposts: CarPost[];
  carmodelautotests: CarModelAutoTest[];

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarPostDataAutoTestService,
    private carModelAutoTestService: CarModelAutoTestService,
    private carPostService: CarPostService) { }

  ngOnInit() {
    this.carpostdataautotestForm = new FormGroup({
      Id: new FormControl(),
      DateTime: new FormControl('', [Validators.required]),
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
      CarPostId: new FormControl('', [Validators.required]),
      CarModelAutoTestId: new FormControl('', [Validators.required]),
      Temperature: new FormControl(''),
      Pressure: new FormControl(''),
      GasSerialNumber: new FormControl(''),
      GasCheckDate: new FormControl(new Date()),
      MeteoSerialNumber: new FormControl(''),
      MeteoCheckDate: new FormControl(new Date()),
      TestNumber: new FormControl(''),
      TesterId: new FormControl(''),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carpostdataautotestForm.patchValue(res as CarPostDataAutoTest);
        this.carpostdataautotestForm.controls["DateTime"].setValue(new Date(res['DateTime']));
        this.carpostdataautotestForm.controls["CarPostId"].setValue(res['CarModelAutoTest']['CarPostId']);
        this.carpostdataautotestForm.controls["GasCheckDate"].setValue(new Date(res['GasCheckDate']));
        this.carpostdataautotestForm.controls["MeteoCheckDate"].setValue(new Date(res['MeteoCheckDate']));
        this.carPostService.get()
          .subscribe(res => {
            this.carposts = res as CarPost[];
            this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
            this.onCarPostChange();
          });
      },
        (error => {
          console.log(error);
        })
    );
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

  public save(carpostdataautotestFormValue) {
    if (this.carpostdataautotestForm.valid) {
      const carpostdataautotest: CarPostDataAutoTest = {
        Id: carpostdataautotestFormValue.Id,
        DateTime: new Date(Date.UTC(carpostdataautotestFormValue.DateTime.getFullYear(), carpostdataautotestFormValue.DateTime.getMonth(), carpostdataautotestFormValue.DateTime.getDate(),
          carpostdataautotestFormValue.DateTime.getHours(), carpostdataautotestFormValue.DateTime.getMinutes(), carpostdataautotestFormValue.DateTime.getSeconds())),
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
        CarModelAutoTestId: carpostdataautotestFormValue.CarModelAutoTestId,
        CarModelAutoTest: null,
        Temperature: carpostdataautotestFormValue.Temperature,
        Pressure: carpostdataautotestFormValue.Pressure,
        GasSerialNumber: carpostdataautotestFormValue.GasSerialNumber,
        GasCheckDate: new Date(Date.UTC(carpostdataautotestFormValue.GasCheckDate.getFullYear(), carpostdataautotestFormValue.GasCheckDate.getMonth(), carpostdataautotestFormValue.GasCheckDate.getDate(),
          carpostdataautotestFormValue.GasCheckDate.getHours(), carpostdataautotestFormValue.GasCheckDate.getMinutes(), carpostdataautotestFormValue.GasCheckDate.getSeconds())),
        MeteoSerialNumber: carpostdataautotestFormValue.MeteoSerialNumber,
        MeteoCheckDate: new Date(Date.UTC(carpostdataautotestFormValue.MeteoCheckDate.getFullYear(), carpostdataautotestFormValue.MeteoCheckDate.getMonth(), carpostdataautotestFormValue.MeteoCheckDate.getDate(),
          carpostdataautotestFormValue.MeteoCheckDate.getHours(), carpostdataautotestFormValue.MeteoCheckDate.getMinutes(), carpostdataautotestFormValue.MeteoCheckDate.getSeconds())),
        TestNumber: carpostdataautotestFormValue.TestNumber,
        TesterId: carpostdataautotestFormValue.TesterId,
        Tester: null,
      }
      this.service.put(carpostdataautotest)
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
