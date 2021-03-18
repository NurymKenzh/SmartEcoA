import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarModelSmokeMeterService } from './carmodelsmokemeter.service';
import { CarModelSmokeMeter } from './carmodelsmokemeter.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'edit.component.html',
  styleUrls: ['edit.component.css']
})

export class CarModelSmokeMeterEditComponent implements OnInit {
  public carmodelsmokemeterForm: FormGroup;
  carposts: CarPost[];

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarModelSmokeMeterService,
    private carpostService: CarPostService) { }

  ngOnInit() {
    this.carpostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      });
    this.carmodelsmokemeterForm = new FormGroup({
      Id: new FormControl(),
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      Boost: new FormControl(''),
      DFreeMark: new FormControl(''),
      //DMaxMark: new FormControl(''),
      CarPostId: new FormControl('', [Validators.required]),
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
        Boost: carmodelsmokemeterFormValue.Boost,
        DFreeMark: carmodelsmokemeterFormValue.DFreeMark,
        DMaxMark: null,
        CarPostId: carmodelsmokemeterFormValue.CarPostId,
        CarPost: null,
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
