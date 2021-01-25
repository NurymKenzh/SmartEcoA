import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { PostDataDividedService } from './postdatadivided.service';
import { PostDataDivided } from './postdatadivided.model';

@Component({
  templateUrl: 'details.component.html'
})

export class PostDataDividedDetailsComponent implements OnInit {
  public postdatadivided: PostDataDivided;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: PostDataDividedService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.postdatadivided = res as PostDataDivided;
      },
        (error => {
          console.log(error);
        })
      );
  }

  public cancel() {
    this.router.navigateByUrl('/postdatadivideds');
  }
}
