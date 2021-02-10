import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { PostDataAvgService } from './postdataavg.service';
import { PostDataAvg } from './postdataavg.model';

@Component({
  templateUrl: 'details.component.html'
})

export class PostDataAvgDetailsComponent implements OnInit {
  public postdataavg: PostDataAvg;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: PostDataAvgService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.postdataavg = res as PostDataAvg;
      },
        (error => {
          console.log(error);
        })
      );
  }

  public cancel() {
    this.router.navigateByUrl('/postdataavgs');
  }
}
