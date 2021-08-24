import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { TesterService } from './tester.service';
import { Tester } from './tester.model';

@Component({
  templateUrl: 'details.component.html'
})

export class TesterDetailsComponent implements OnInit {
  public tester: Tester;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: TesterService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.tester = res as Tester;
      },
        (error => {
          console.log(error);
        })
    );
  }

  public cancel() {
    this.router.navigateByUrl('/testers');
  }
}
