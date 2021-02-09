import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarModelSmokeMeterService } from './carmodelsmokemeter.service';
import { CarModelSmokeMeter } from './carmodelsmokemeter.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'create.component.html',
  styleUrls: ['create.component.css']
})

export class CarModelSmokeMeterCreateComponent implements OnInit {
  public carmodelsmokemeterForm: FormGroup;
  carposts: CarPost[];

  constructor(private router: Router,
    private service: CarModelSmokeMeterService,
    private carPostService: CarPostService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.carmodelsmokemeterForm.controls["CarPostId"].setValue(this.carposts[0].Id);
      });
    this.carmodelsmokemeterForm = new FormGroup({
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      Boost: new FormControl(false, [Validators.required]),
      DFreeMark: new FormControl(''),
      //DMaxMark: new FormControl(''),
      CarPostId: new FormControl('', [Validators.required]),
    });
  }

  public error(control: string,
    error: string) {
    return this.carmodelsmokemeterForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carmodelsmokemeters');
  }

  public create(carmodelsmokemeterFormValue) {
    if (this.carmodelsmokemeterForm.valid) {
      const carmodelsmokemeter: CarModelSmokeMeter = {
        Id: 0,
        Name: carmodelsmokemeterFormValue.Name,
        Boost: carmodelsmokemeterFormValue.Boost,
        DFreeMark: carmodelsmokemeterFormValue.DFreeMark,
        DMaxMark: null,
        CarPostId: carmodelsmokemeterFormValue.CarPostId,
        CarPost: null,
      }
      this.service.post(carmodelsmokemeter)
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
