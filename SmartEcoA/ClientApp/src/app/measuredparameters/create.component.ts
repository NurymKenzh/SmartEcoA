import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { MeasuredParameterService } from './measuredparameter.service';
import { MeasuredParameter } from './measuredparameter.model';

@Component({
  templateUrl: 'create.component.html'
})

export class MeasuredParameterCreateComponent implements OnInit {
  public measuredparameterForm: FormGroup;

  constructor(private router: Router,
    private service: MeasuredParameterService) { }

  ngOnInit() {
    this.measuredparameterForm = new FormGroup({
      NameEN: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      NameRU: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      NameKK: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      MPCDailyAverage: new FormControl(''),
      MPCMaxOneTime: new FormControl(''),
      OceanusCode: new FormControl(''),
      KazhydrometCode: new FormControl(''),
    });
  }

  public error(control: string,
    error: string) {
    return this.measuredparameterForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/measuredparameters');
  }

  public create(measuredparameterFormValue) {
    if (this.measuredparameterForm.valid) {
      const measuredparameter: MeasuredParameter = {
        Id: 0,
        NameEN: measuredparameterFormValue.NameEN,
        NameRU: measuredparameterFormValue.NameRU,
        NameKK: measuredparameterFormValue.NameKK,
        Name: '',
        MPCDailyAverage: measuredparameterFormValue.MPCDailyAverage,
        MPCMaxOneTime: measuredparameterFormValue.MPCMaxOneTime,
        OceanusCode: measuredparameterFormValue.OceanusCode,
        KazhydrometCode: measuredparameterFormValue.KazhydrometCode
      }
      this.service.post(measuredparameter)
        .subscribe(() => {
          this.router.navigateByUrl('/measuredparameters');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
