import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ProjectService } from './project.service';
import { Project } from './project.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class ProjectEditComponent implements OnInit {
  public projectForm: FormGroup;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: ProjectService) { }

  ngOnInit() {
    this.projectForm = new FormGroup({
      Id: new FormControl(),
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.projectForm.patchValue(res as Project);
      },
        (error => {
          console.log(error);
        })
      );
  }

  public error(control: string,
    error: string) {
    return this.projectForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/projects');
  }

  public save(projectFormValue) {
    if (this.projectForm.valid) {
      const project: Project = {
        Id: projectFormValue.Id,
        Name: projectFormValue.Name
      }
      this.service.put(project)
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
