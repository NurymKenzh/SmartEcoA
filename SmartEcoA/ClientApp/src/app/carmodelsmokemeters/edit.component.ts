import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarModelSmokeMeterService } from './carmodelsmokemeter.service';
import { CarModelSmokeMeter } from './carmodelsmokemeter.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

import { TypeEcoClassService } from '../typeecoclasses/typeecoclass.service';
import { TypeEcoClass } from '../typeecoclasses/typeecoclass.model';

@Component({
  templateUrl: 'edit.component.html',
  styleUrls: ['edit.component.css']
})

export class CarModelSmokeMeterEditComponent implements OnInit {
  public carmodelsmokemeterForm: FormGroup;
  carposts: CarPost[];
  typeecoclasses: TypeEcoClass[];

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarModelSmokeMeterService,
    private carpostService: CarPostService,
    private typeEcoClassService: TypeEcoClassService) { }

  ngOnInit() {
    this.carpostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      });
    this.typeEcoClassService.get()
      .subscribe(res => {
        this.typeecoclasses = res as TypeEcoClass[];
        this.typeecoclasses.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      });
    this.carmodelsmokemeterForm = new FormGroup({
      Id: new FormControl(),
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      TypeEcoClassId: new FormControl('', [Validators.required]),
      Category: new FormControl(''),
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
      ParadoxId: new FormControl(''),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carmodelsmokemeterForm.patchValue(res as CarModelSmokeMeter);
      },
        (error => {
          console.log(error);
        })
    );
  }

  public error(control: string,
    error: string) {
    return this.carmodelsmokemeterForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carmodelsmokemeters');
  }

  public save(carmodelsmokemeterFormValue) {
    if (this.carmodelsmokemeterForm.valid) {
      const carmodelsmokemeter: CarModelSmokeMeter = {
        Id: carmodelsmokemeterFormValue.Id,
        Name: carmodelsmokemeterFormValue.Name,
        TypeEcoClassId: carmodelsmokemeterFormValue.TypeEcoClassId,
        TypeEcoClass: null,
        Category: carmodelsmokemeterFormValue.Category,
        EngineType: carmodelsmokemeterFormValue.EngineType,
        MIN_TAH: carmodelsmokemeterFormValue.MIN_TAH,
        DEL_MIN: carmodelsmokemeterFormValue.DEL_MIN,
        MAX_TAH: carmodelsmokemeterFormValue.MAX_TAH,
        DEL_MAX: carmodelsmokemeterFormValue.DEL_MAX,
        MIN_CO: carmodelsmokemeterFormValue.MIN_CO,
        MAX_CO: carmodelsmokemeterFormValue.MAX_CO,
        MIN_CH: carmodelsmokemeterFormValue.MIN_CH,
        MAX_CH: carmodelsmokemeterFormValue.MAX_CH,
        L_MIN: carmodelsmokemeterFormValue.L_MIN,
        L_MAX: carmodelsmokemeterFormValue.L_MAX,
        K_SVOB: carmodelsmokemeterFormValue.K_SVOB,
        K_MAX: carmodelsmokemeterFormValue.K_MAX,
        CarPostId: carmodelsmokemeterFormValue.CarPostId,
        CarPost: null,
        ParadoxId: carmodelsmokemeterFormValue.ParadoxId,
      }
      this.service.put(carmodelsmokemeter)
        .subscribe(() => {
          this.router.navigateByUrl('/carmodelsmokemeters');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
