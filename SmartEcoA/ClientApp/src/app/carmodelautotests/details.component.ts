import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { CarModelAutoTestService } from './carmodelautotest.service';
import { CarModelAutoTest } from './carmodelautotest.model';

@Component({
  templateUrl: 'details.component.html'
})

export class CarModelAutoTestDetailsComponent implements OnInit {
  public carmodelautotest: CarModelAutoTest;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarModelAutoTestService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carmodelautotest = res as CarModelAutoTest;
      },
        (error => {
          console.log(error);
        })
    );
  }

  public cancel() {
    this.router.navigateByUrl('/carmodelautotests');
  }
}
