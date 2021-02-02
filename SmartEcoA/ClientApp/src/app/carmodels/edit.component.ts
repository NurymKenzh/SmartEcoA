import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarModelService } from './carmodel.service';
import { CarModel } from './carmodel.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class CarModelEditComponent implements OnInit {
  public carmodelForm: FormGroup;
  carposts: CarPost[];

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarModelService,
    private carpostService: CarPostService) { }

  ngOnInit() {
    this.carpostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      });
    this.carmodelForm = new FormGroup({
      Id: new FormControl(),
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      Boost: new FormControl(''),
      DFreeMark: new FormControl(''),
      DMaxMark: new FormControl(''),
      CarPostId: new FormControl('', [Validators.required]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carmodelForm.patchValue(res as CarModel);
      },
        (error => {
          console.log(error);
        })
    );
  }

  public error(control: string,
    error: string) {
    return this.carmodelForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carmodels');
  }

  public save(carmodelFormValue) {
    if (this.carmodelForm.valid) {
      const carmodel: CarModel = {
        Id: carmodelFormValue.Id,
        Name: carmodelFormValue.Name,
        Boost: carmodelFormValue.Boost,
        DFreeMark: carmodelFormValue.DFreeMark,
        DMaxMark: carmodelFormValue.DMaxMark,
        CarPostId: carmodelFormValue.CarPostId,
        CarPost: null,
      }
      this.service.put(carmodel)
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
