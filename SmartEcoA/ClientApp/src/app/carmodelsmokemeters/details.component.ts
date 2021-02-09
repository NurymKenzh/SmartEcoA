import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { CarModelSmokeMeterService } from './carmodelsmokemeter.service';
import { CarModelSmokeMeter } from './carmodelsmokemeter.model';

@Component({
  templateUrl: 'details.component.html'
})

export class CarModelSmokeMeterDetailsComponent implements OnInit {
  public carmodelsmokemeter: CarModelSmokeMeter;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarModelSmokeMeterService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carmodelsmokemeter = res as CarModelSmokeMeter;
      },
        (error => {
          console.log(error);
        })
    );
  }

  public cancel() {
    this.router.navigateByUrl('/carmodelsmokemeters');
  }
}
