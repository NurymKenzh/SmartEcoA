import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarModelAutoTestService } from './carmodelautotest.service';
import { CarModelAutoTest } from './carmodelautotest.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class CarModelAutoTestEditComponent implements OnInit {
  public carmodelautotestForm: FormGroup;
  carposts: CarPost[];

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarModelAutoTestService,
    private carpostService: CarPostService) { }

  ngOnInit() {
    this.carpostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      });
    this.carmodelautotestForm = new FormGroup({
      Id: new FormControl(),
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      EngineType: new FormControl(''),
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
      CarPostId: new FormControl('', [Validators.required]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carmodelautotestForm.patchValue(res as CarModelAutoTest);
      },
        (error => {
          console.log(error);
        })
    );
  }

  public error(control: string,
    error: string) {
    return this.carmodelautotestForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carmodelautotests');
  }

  public save(carmodelautotestFormValue) {
    if (this.carmodelautotestForm.valid) {
      const carmodelautotest: CarModelAutoTest = {
        Id: carmodelautotestFormValue.Id,
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
        CarPostId: carmodelautotestFormValue.CarPostId,
        CarPost: null,
      }
      this.service.put(carmodelautotest)
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
