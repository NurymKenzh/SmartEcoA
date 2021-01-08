import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { MeasuredParameterService } from './measuredparameter.service';
import { MeasuredParameter } from './measuredparameter.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class MeasuredParameterEditComponent implements OnInit {
  public measuredparameterForm: FormGroup;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: MeasuredParameterService) { }

  ngOnInit() {
    this.measuredparameterForm = new FormGroup({
      Id: new FormControl(),
      NameEN: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      NameRU: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      NameKK: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      MPCDailyAverage: new FormControl(''),
      MPCMaxOneTime: new FormControl(''),
      OceanusCode: new FormControl(''),
      OceanusCoefficient: new FormControl('', [Validators.required]),
      KazhydrometCode: new FormControl(''),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.measuredparameterForm.patchValue(res as MeasuredParameter);
      },
        (error => {
          console.log(error);
        })
      );
  }

  public error(control: string,
    error: string) {
    return this.measuredparameterForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/measuredparameters');
  }

  public save(measuredparameterFormValue) {
    if (this.measuredparameterForm.valid) {
      const measuredparameter: MeasuredParameter = {
        Id: measuredparameterFormValue.Id,
        NameEN: measuredparameterFormValue.NameEN,
        NameRU: measuredparameterFormValue.NameRU,
        NameKK: measuredparameterFormValue.NameKK,
        Name: '',
        MPCDailyAverage: measuredparameterFormValue.MPCDailyAverage,
        MPCMaxOneTime: measuredparameterFormValue.MPCMaxOneTime,
        OceanusCode: measuredparameterFormValue.OceanusCode,
        OceanusCoefficient: measuredparameterFormValue.OceanusCoefficient,
        KazhydrometCode: measuredparameterFormValue.KazhydrometCode
      }
      this.service.put(measuredparameter)
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
