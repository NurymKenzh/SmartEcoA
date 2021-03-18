import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { CarPostDataAutoTestService } from './carpostdataautotest.service';
import { CarPostDataAutoTest } from './carpostdataautotest.model';

@Component({
  templateUrl: 'details.component.html'
})

export class CarPostDataAutoTestDetailsComponent implements OnInit {
  public carpostdataautotest: CarPostDataAutoTest;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarPostDataAutoTestService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carpostdataautotest = res as CarPostDataAutoTest;
      },
        (error => {
          console.log(error);
        })
    );
  }

  public cancel() {
    this.router.navigateByUrl('/carpostdataautotests');
  }
}
