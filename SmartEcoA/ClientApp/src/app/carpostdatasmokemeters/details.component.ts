import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { CarPostDataSmokeMeterService } from './carpostdatasmokemeter.service';
import { CarPostDataSmokeMeter } from './carpostdatasmokemeter.model';

@Component({
  templateUrl: 'details.component.html'
})

export class CarPostDataSmokeMeterDetailsComponent implements OnInit {
  public carpostdatasmokemeter: CarPostDataSmokeMeter;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarPostDataSmokeMeterService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carpostdatasmokemeter = res as CarPostDataSmokeMeter;
      },
        (error => {
          console.log(error);
        })
    );
  }

  public cancel() {
    this.router.navigateByUrl('/carpostdatasmokemeters');
  }
}
