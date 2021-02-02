import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarModelService } from './carmodel.service';
import { CarModel } from './carmodel.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'create.component.html'
})

export class CarModelCreateComponent implements OnInit {
  public carmodelForm: FormGroup;
  carposts: CarPost[];

  constructor(private router: Router,
    private service: CarModelService,
    private carPostService: CarPostService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0)); 
      });
    this.carmodelForm = new FormGroup({
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      Boost: new FormControl(''),
      DFreeMark: new FormControl(''),
      DMaxMark: new FormControl(''),
      CarPostId: new FormControl('', [Validators.required]),
    });
  }

  public error(control: string,
    error: string) {
    return this.carmodelForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carmodels');
  }

  public create(carmodelFormValue) {
    if (this.carmodelForm.valid) {
      const carmodel: CarModel = {
        Id: 0,
        Name: carmodelFormValue.Name,
        Boost: carmodelFormValue.Boost,
        DFreeMark: carmodelFormValue.DFreeMark,
        DMaxMark: carmodelFormValue.DMaxMark,
        CarPostId: carmodelFormValue.CarPostId,
        CarPost: null,
      }
      this.service.post(carmodel)
        .subscribe(() => {
          this.router.navigateByUrl('/carmodels');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
