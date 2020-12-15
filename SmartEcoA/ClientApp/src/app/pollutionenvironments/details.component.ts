import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { PollutionEnvironmentService } from './pollutionenvironment.service';
import { PollutionEnvironment } from './pollutionenvironment.model';

@Component({
  templateUrl: 'details.component.html'
})

export class PollutionEnvironmentDetailsComponent implements OnInit {
  public pollutionenvironment: PollutionEnvironment;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: PollutionEnvironmentService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.pollutionenvironment = res as PollutionEnvironment;
      },
        (error => {
          console.log(error);
        })
      );
  }

  public cancel() {
    this.router.navigateByUrl('/pollutionenvironments');
  }
}
