import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { PostDataService } from './postdata.service';
import { PostData } from './postdata.model';

@Component({
  templateUrl: 'details.component.html'
})

export class PostDataDetailsComponent implements OnInit {
  public postdata: PostData;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: PostDataService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.postdata = res as PostData;
      },
        (error => {
          console.log(error);
        })
      );
  }

  public cancel() {
    this.router.navigateByUrl('/postdatas');
  }
}
