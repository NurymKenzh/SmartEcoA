import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { CarPostAnalyticService } from './carpostanalytic.service';
import { CarPostAnalytic } from './carpostanalytic.model';

@Component({
  templateUrl: 'details.component.html'
})

export class CarPostAnalyticDetailsComponent implements OnInit {
  public carpostanalytic: CarPostAnalytic;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarPostAnalyticService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carpostanalytic = res as CarPostAnalytic;
      },
        (error => {
          console.log(error);
        })
    );
  }

  public cancel() {
    this.router.navigateByUrl('/carpostanalytics');
  }
}
