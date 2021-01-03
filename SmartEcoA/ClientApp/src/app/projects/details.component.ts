import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { ProjectService } from './project.service';
import { Project } from './project.model';

@Component({
  templateUrl: 'details.component.html'
})

export class ProjectDetailsComponent implements OnInit {
  public project: Project;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: ProjectService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.project = res as Project;
      },
        (error => {
          console.log(error);
        })
      );
  }

  public cancel() {
    this.router.navigateByUrl('/projects');
  }
}
