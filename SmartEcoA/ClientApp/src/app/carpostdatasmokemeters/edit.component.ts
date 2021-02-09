import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarPostDataSmokeMeterService } from './carpostdatasmokemeter.service';
import { CarPostDataSmokeMeter } from './carpostdatasmokemeter.model';

import { CarModelSmokeMeterService } from '../carmodelsmokemeters/carmodelsmokemeter.service';
import { CarModelSmokeMeter } from '../carmodelsmokemeters/carmodelsmokemeter.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class CarPostDataSmokeMeterEditComponent implements OnInit {
  public carpostdatasmokemeterForm: FormGroup;
  carposts: CarPost[];
  carmodelsmokemeters: CarModelSmokeMeter[];

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarPostDataSmokeMeterService,
    private carModelSmokeMeterService: CarModelSmokeMeterService,
    private carPostService: CarPostService) { }

  ngOnInit() {
    this.carpostdatasmokemeterForm = new FormGroup({
      Id: new FormControl(),
      DateTime: new FormControl('', [Validators.required]),
      Number: new FormControl('', [Validators.maxLength(10)]),
      RunIn: new FormControl('', [Validators.required]),
      DFree: new FormControl('', [Validators.required]),
      NDFree: new FormControl('', [Validators.required]),
      CarPostId: new FormControl('', [Validators.required]),
      CarModelSmokeMeterId: new FormControl('', [Validators.required]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carpostdatasmokemeterForm.patchValue(res as CarPostDataSmokeMeter);
        this.carpostdatasmokemeterForm.controls["DateTime"].setValue(new Date(res['DateTime']));
        this.carpostdatasmokemeterForm.controls["CarPostId"].setValue(res['CarModelSmokeMeter']['CarPostId']);
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

  public save(carpostdatasmokemeterFormValue) {
    if (this.carpostdatasmokemeterForm.valid) {
      const carpostdatasmokemeter: CarPostDataSmokeMeter = {
        Id: carpostdatasmokemeterFormValue.Id,
        DateTime: carpostdatasmokemeterFormValue.DateTime.toLocaleString(),
        Number: carpostdatasmokemeterFormValue.Number,
        RunIn: carpostdatasmokemeterFormValue.RunIn,
        DFree: carpostdatasmokemeterFormValue.DFree,
        DMax: 0,
        NDFree: carpostdatasmokemeterFormValue.NDFree,
        NDMax: 0,
        CarModelSmokeMeterId: carpostdatasmokemeterFormValue.CarModelSmokeMeterId,
        CarModelSmokeMeter: null,
      }
      this.service.put(carpostdatasmokemeter)
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
