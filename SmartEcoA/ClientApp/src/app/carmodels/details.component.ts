import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { CarModelService } from './carmodel.service';
import { CarModel } from './carmodel.model';

@Component({
  templateUrl: 'details.component.html'
})

export class CarModelDetailsComponent implements OnInit {
  public carmodel: CarModel;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarModelService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carmodel = res as CarModel;
      },
        (error => {
          console.log(error);
        })
    );
  }

  public cancel() {
    this.router.navigateByUrl('/carmodels');
  }
}
