import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { MeasuredParameterService } from './measuredparameter.service';
import { MeasuredParameter } from './measuredparameter.model';

@Component({
  templateUrl: 'details.component.html'
})

export class MeasuredParameterDetailsComponent implements OnInit {
  public measuredparameter: MeasuredParameter;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: MeasuredParameterService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.measuredparameter = res as MeasuredParameter;
      },
        (error => {
          console.log(error);
        })
      );
  }

  public cancel() {
    this.router.navigateByUrl('/measuredparameters');
  }
}
