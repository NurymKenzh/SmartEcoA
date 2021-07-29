import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { ReportService } from './report.service';
import { Report } from './report.model';

@Component({
  templateUrl: 'details.component.html',
  styleUrls: ['details.component.css'],
})

export class ReportDetailsComponent implements OnInit {
  public report: Report;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: ReportService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.report = res as Report;
      },
        (error => {
          console.log(error);
        })
      );
  }

  public cancel() {
    this.router.navigateByUrl('/reports');
  }

  public download(Id, FileName) {
    this.service.download(Id, FileName);
  }
}
