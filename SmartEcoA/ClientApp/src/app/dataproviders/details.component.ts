import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { DataProviderService } from './dataprovider.service';
import { DataProvider } from './dataprovider.model';

@Component({
  templateUrl: 'details.component.html'
})

export class DataProviderDetailsComponent implements OnInit {
  public dataprovider: DataProvider;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: DataProviderService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.dataprovider = res as DataProvider;
      },
        (error => {
          console.log(error);
        })
      );
  }

  public cancel() {
    this.router.navigateByUrl('/dataproviders');
  }
}
