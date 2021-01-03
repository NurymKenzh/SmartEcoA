import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ProjectService } from './project.service';
import { Project } from './project.model';

@Component({
  templateUrl: 'create.component.html'
})

export class ProjectCreateComponent implements OnInit {
  public projectForm: FormGroup;

  constructor(private router: Router,
    private service: ProjectService) { }

  ngOnInit() {
    this.projectForm = new FormGroup({
      Name: new FormControl('', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z]*')]),
    });
  }

  public error(control: string,
    error: string) {
    return this.projectForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/projects');
  }

  public create(projectFormValue) {
    if (this.projectForm.valid) {
      const project: Project = {
        Id: 0,
        Name: projectFormValue.Name
      }
      this.service.post(project)
        .subscribe(() => {
          this.router.navigateByUrl('/projects');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
