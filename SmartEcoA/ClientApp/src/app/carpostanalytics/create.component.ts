import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarPostAnalyticService } from './carpostanalytic.service';
import { CarPostAnalytic } from './carpostanalytic.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'create.component.html'
})

export class CarPostAnalyticCreateComponent implements OnInit {
  public carpostanalyticForm: FormGroup;
  carposts: CarPost[];

  constructor(private router: Router,
    private service: CarPostAnalyticService,
    private carPostService: CarPostService) { }

  ngOnInit() {
    this.carPostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.carpostanalyticForm.controls["CarPostId"].setValue(this.carposts[0].Id);
      });
    this.carpostanalyticForm = new FormGroup({
      Date: new FormControl(new Date(), [Validators.required]),
      Measurement: new FormControl('', [Validators.required]),
      Exceeding: new FormControl('', [Validators.required]),
      CarPostId: new FormControl('', [Validators.required]),
    });
  }

  public error(control: string,
    error: string) {
    return this.carpostanalyticForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carpostanalytics');
  }

  public create(carpostanalyticFormValue) {
    if (this.carpostanalyticForm.valid) {
      const carpostanalytic: CarPostAnalytic = {
        Id: 0,
        Date: carpostanalyticFormValue.Date.toLocaleString(),
        Measurement: carpostanalyticFormValue.Measurement,
        Exceeding: carpostanalyticFormValue.Exceeding,
        CarPostId: carpostanalyticFormValue.CarPostId,
        CarPost: null,
      }
      this.service.post(carpostanalytic)
        .subscribe(() => {
          this.router.navigateByUrl('/carpostanalytics');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
