import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { CarPostAnalyticService } from './carpostanalytic.service';
import { CarPostAnalytic } from './carpostanalytic.model';

import { CarPostService } from '../carposts/carpost.service';
import { CarPost } from '../carposts/carpost.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class CarPostAnalyticEditComponent implements OnInit {
  public carpostanalyticForm: FormGroup;
  carposts: CarPost[];

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarPostAnalyticService,
    private carpostService: CarPostService) { }

  ngOnInit() {
    this.carpostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      });
    this.carpostanalyticForm = new FormGroup({
      Id: new FormControl(),
      Date: new FormControl('', [Validators.required]),
      Measurement: new FormControl('', [Validators.required]),
      Exceeding: new FormControl('', [Validators.required]),
      CarPostId: new FormControl('', [Validators.required]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carpostanalyticForm.patchValue(res as CarPostAnalytic);
        this.carpostanalyticForm.controls["Date"].setValue(new Date(res['Date']));
      },
        (error => {
          console.log(error);
        })
    );
  }

  public error(control: string,
    error: string) {
    return this.carpostanalyticForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carpostanalytics');
  }

  public save(carpostanalyticFormValue) {
    if (this.carpostanalyticForm.valid) {
      const carpostanalytic: CarPostAnalytic = {
        Id: carpostanalyticFormValue.Id,
        Date: carpostanalyticFormValue.Date.toLocaleString(),
        Measurement: carpostanalyticFormValue.Measurement,
        Exceeding: carpostanalyticFormValue.Exceeding,
        CarPostId: carpostanalyticFormValue.CarPostId,
        CarPost: null,
      }
      this.service.put(carpostanalytic)
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
