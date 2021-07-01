import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarModelAutoTestService } from './carmodelautotest.service';
import { CarModelAutoTest } from './carmodelautotest.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'create.component.html',
  styleUrls: ['create.component.css']
})

export class CarModelAutoTestCreateComponent implements OnInit {
  public carmodelautotestForm: FormGroup;
  carposts: CarPost[];

  constructor(private router: Router,
    private service: CarModelAutoTestService,
    private carPostService: CarPostService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.carmodelautotestForm.controls["CarPostId"].setValue(this.carposts[0].Id);
      });
    this.carmodelautotestForm = new FormGroup({
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      EngineType: new FormControl(0),
      MIN_TAH: new FormControl(''),
      DEL_MIN: new FormControl(''),
      MAX_TAH: new FormControl(''),
      DEL_MAX: new FormControl(''),
      MIN_CO: new FormControl(''),
      MAX_CO: new FormControl(''),
      MIN_CH: new FormControl(''),
      MAX_CH: new FormControl(''),
      L_MIN: new FormControl(''),
      L_MAX: new FormControl(''),
      K_SVOB: new FormControl(''),
      K_MAX: new FormControl(''),
      MIN_CO2: new FormControl(''),
      MIN_O2: new FormControl(''),
      MIN_NOx: new FormControl(''),
      MAX_CO2: new FormControl(''),
      MAX_O2: new FormControl(''),
      MAX_NOx: new FormControl(''),
      Version: new FormControl(''),
      CarPostId: new FormControl('', [Validators.required]),
    });
  }

  public error(control: string,
    error: string) {
    return this.carmodelautotestForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carmodelautotests');
  }

  public create(carmodelautotestFormValue) {
    if (this.carmodelautotestForm.valid) {
      const carmodelautotest: CarModelAutoTest = {
        Id: 0,
        Name: carmodelautotestFormValue.Name,
        EngineType: carmodelautotestFormValue.EngineType,
        MIN_TAH: carmodelautotestFormValue.MIN_TAH,
        DEL_MIN: carmodelautotestFormValue.DEL_MIN,
        MAX_TAH: carmodelautotestFormValue.MAX_TAH,
        DEL_MAX: carmodelautotestFormValue.DEL_MAX,
        MIN_CO: carmodelautotestFormValue.MIN_CO,
        MAX_CO: carmodelautotestFormValue.MAX_CO,
        MIN_CH: carmodelautotestFormValue.MIN_CH,
        MAX_CH: carmodelautotestFormValue.MAX_CH,
        L_MIN: carmodelautotestFormValue.L_MIN,
        L_MAX: carmodelautotestFormValue.L_MAX,
        K_SVOB: carmodelautotestFormValue.K_SVOB,
        K_MAX: carmodelautotestFormValue.K_MAX,
        MIN_CO2: carmodelautotestFormValue.MIN_CO2,
        MIN_O2: carmodelautotestFormValue.MIN_O2,
        MIN_NOx: carmodelautotestFormValue.MIN_NOx,
        MAX_CO2: carmodelautotestFormValue.MAX_CO2,
        MAX_O2: carmodelautotestFormValue.MAX_O2,
        MAX_NOx: carmodelautotestFormValue.MAX_NOx,
        Version: carmodelautotestFormValue.Version,
        CarPostId: carmodelautotestFormValue.CarPostId,
        CarPost: null,
      }
      this.service.post(carmodelautotest)
        .subscribe(() => {
          this.router.navigateByUrl('/carmodelautotests');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
