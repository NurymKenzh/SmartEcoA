import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarPostDataSmokeMeterService } from './carpostdatasmokemeter.service';
import { CarPostDataSmokeMeter } from './carpostdatasmokemeter.model';

import { CarModelSmokeMeterService } from '../carmodelsmokemeters/carmodelsmokemeter.service';
import { CarModelSmokeMeter } from '../carmodelsmokemeters/carmodelsmokemeter.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

import { Tester } from '../testers/tester.model';
import { TesterService } from '../testers/tester.service';

@Component({
  templateUrl: 'create.component.html'
})

export class CarPostDataSmokeMeterCreateComponent implements OnInit, AfterViewInit {
  public carpostdatasmokemeterForm: FormGroup;
  carposts: CarPost[];
  carmodelsmokemeters: CarModelSmokeMeter[];
  testers: Tester[];

  constructor(private router: Router,
    private service: CarPostDataSmokeMeterService,
    private carModelSmokeMeterService: CarModelSmokeMeterService,
    private carPostService: CarPostService,
    private testerService: TesterService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.carpostdatasmokemeterForm.controls["CarPostId"].setValue(this.carposts[0] ? this.carposts[0].Id : null);
        this.onCarPostChange();
      });
    this.testerService.get()
      .subscribe(res => {
        this.testers = res as Tester[];
        this.testers.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.carpostdatasmokemeterForm.controls["TesterId"].setValue(this.testers[0].Id);
      });
    this.carpostdatasmokemeterForm = new FormGroup({
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
      CarPostId: new FormControl('', [Validators.required]),
      CarModelSmokeMeterId: new FormControl('', [Validators.required]),
      Temperature: new FormControl(''),
      Pressure: new FormControl(''),
      GasSerialNumber: new FormControl(''),
      GasCheckDate: new FormControl(new Date()),
      MeteoSerialNumber: new FormControl(''),
      MeteoCheckDate: new FormControl(new Date()),
      TestNumber: new FormControl(''),
      TesterId: new FormControl(''),
    });
  }

  ngAfterViewInit() {
    this.carpostdatasmokemeterForm.controls["DateTime"].setValue(new Date());
  }

  public onCarPostChange() {
    this.carModelSmokeMeterService.get(null, this.carpostdatasmokemeterForm.controls["CarPostId"].value)
      .subscribe(res => {
        this.carmodelsmokemeters = res as CarModelSmokeMeter[];
        this.carmodelsmokemeters.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.carpostdatasmokemeterForm.controls["CarModelSmokeMeterId"].setValue(this.carmodelsmokemeters[0] ? this.carmodelsmokemeters[0].Id : null);
      });
  }

  public error(control: string,
    error: string) {
    return this.carpostdatasmokemeterForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carpostdatasmokemeters');
  }

  public create(carpostdatasmokemeterFormValue) {
    if (this.carpostdatasmokemeterForm.valid) {
      const carpostdatasmokemeter: CarPostDataSmokeMeter = {
        Id: 0,
        DateTime: new Date(Date.UTC(carpostdatasmokemeterFormValue.DateTime.getFullYear(), carpostdatasmokemeterFormValue.DateTime.getMonth(), carpostdatasmokemeterFormValue.DateTime.getDate(),
          carpostdatasmokemeterFormValue.DateTime.getHours(), carpostdatasmokemeterFormValue.DateTime.getMinutes(), carpostdatasmokemeterFormValue.DateTime.getSeconds())),
        Number: carpostdatasmokemeterFormValue.Number,
        DOPOL1: carpostdatasmokemeterFormValue.DOPOL1,
        DOPOL2: carpostdatasmokemeterFormValue.DOPOL2,
        MIN_TAH: carpostdatasmokemeterFormValue.MIN_TAH,
        MIN_CO: carpostdatasmokemeterFormValue.MIN_CO,
        MIN_CH: carpostdatasmokemeterFormValue.MIN_CH,
        MIN_CO2: carpostdatasmokemeterFormValue.MIN_CO2,
        MIN_O2: carpostdatasmokemeterFormValue.MIN_O2,
        MIN_L: carpostdatasmokemeterFormValue.MIN_L,
        MAX_TAH: carpostdatasmokemeterFormValue.MAX_TAH,
        MAX_CO: carpostdatasmokemeterFormValue.MAX_CO,
        MAX_CH: carpostdatasmokemeterFormValue.MAX_CH,
        MAX_CO2: carpostdatasmokemeterFormValue.MAX_CO2,
        MAX_O2: carpostdatasmokemeterFormValue.MAX_O2,
        MAX_L: carpostdatasmokemeterFormValue.MAX_L,
        ZAV_NOMER: carpostdatasmokemeterFormValue.ZAV_NOMER,
        K_1: carpostdatasmokemeterFormValue.K_1,
        K_2: carpostdatasmokemeterFormValue.K_2,
        K_3: carpostdatasmokemeterFormValue.K_3,
        K_4: carpostdatasmokemeterFormValue.K_4,
        K_SVOB: carpostdatasmokemeterFormValue.K_SVOB,
        K_MAX: carpostdatasmokemeterFormValue.K_MAX,
        MIN_NO: carpostdatasmokemeterFormValue.MIN_NO,
        MAX_NO: carpostdatasmokemeterFormValue.MAX_NO,
        CarModelSmokeMeterId: carpostdatasmokemeterFormValue.CarModelSmokeMeterId,
        CarModelSmokeMeter: null,
        Temperature: carpostdatasmokemeterFormValue.Temperature,
        Pressure: carpostdatasmokemeterFormValue.Pressure,
        GasSerialNumber: carpostdatasmokemeterFormValue.GasSerialNumber,
        GasCheckDate: new Date(Date.UTC(carpostdatasmokemeterFormValue.GasCheckDate.getFullYear(), carpostdatasmokemeterFormValue.GasCheckDate.getMonth(), carpostdatasmokemeterFormValue.GasCheckDate.getDate(),
          carpostdatasmokemeterFormValue.GasCheckDate.getHours(), carpostdatasmokemeterFormValue.GasCheckDate.getMinutes(), carpostdatasmokemeterFormValue.GasCheckDate.getSeconds())),
        MeteoSerialNumber: carpostdatasmokemeterFormValue.MeteoSerialNumber,
        MeteoCheckDate: new Date(Date.UTC(carpostdatasmokemeterFormValue.MeteoCheckDate.getFullYear(), carpostdatasmokemeterFormValue.MeteoCheckDate.getMonth(), carpostdatasmokemeterFormValue.MeteoCheckDate.getDate(),
          carpostdatasmokemeterFormValue.MeteoCheckDate.getHours(), carpostdatasmokemeterFormValue.MeteoCheckDate.getMinutes(), carpostdatasmokemeterFormValue.MeteoCheckDate.getSeconds())),
        TestNumber: carpostdatasmokemeterFormValue.TestNumber,
        TesterId: carpostdatasmokemeterFormValue.TesterId,
        Tester: null,
      }
      this.service.post(carpostdatasmokemeter)
        .subscribe(() => {
          this.router.navigateByUrl('/carpostdatasmokemeters');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
