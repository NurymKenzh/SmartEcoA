import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { PollutionEnvironmentService } from './pollutionenvironment.service';
import { PollutionEnvironment } from './pollutionenvironment.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class PollutionEnvironmentEditComponent implements OnInit {
  public pollutionenvironmentForm: FormGroup;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: PollutionEnvironmentService) { }

  ngOnInit() {
    this.pollutionenvironmentForm = new FormGroup({
      Id: new FormControl(),
      NameEN: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      NameRU: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      NameKK: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.pollutionenvironmentForm.patchValue(res as PollutionEnvironment);
      },
        (error => {
          console.log(error);
        })
      );
  }

  public error(control: string,
    error: string) {
    return this.pollutionenvironmentForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/pollutionenvironments');
  }

  public save(pollutionenvironmentFormValue) {
    if (this.pollutionenvironmentForm.valid) {
      const pollutionenvironment: PollutionEnvironment = {
        Id: pollutionenvironmentFormValue.Id,
        NameEN: pollutionenvironmentFormValue.NameEN,
        NameRU: pollutionenvironmentFormValue.NameRU,
        NameKK: pollutionenvironmentFormValue.NameKK,
        Name: ''
      }
      this.service.put(pollutionenvironment)
        .subscribe(() => {
          this.router.navigateByUrl('/pollutionenvironments');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
