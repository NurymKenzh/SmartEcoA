import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { PollutionEnvironmentService } from './pollutionenvironment.service';
import { PollutionEnvironment } from './pollutionenvironment.model';

@Component({
  templateUrl: 'create.component.html'
})

export class PollutionEnvironmentCreateComponent implements OnInit {
  public pollutionenvironmentForm: FormGroup;

  constructor(private router: Router,
    private service: PollutionEnvironmentService) { }

  ngOnInit() {
    this.pollutionenvironmentForm = new FormGroup({
      NameEN: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      NameRU: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      NameKK: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    });
  }

  public error(control: string,
    error: string) {
    return this.pollutionenvironmentForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/pollutionenvironments');
  }

  public create(pollutionenvironmentFormValue) {
    if (this.pollutionenvironmentForm.valid) {
      const pollutionenvironment: PollutionEnvironment = {
        Id: 0,
        NameEN: pollutionenvironmentFormValue.NameEN,
        NameRU: pollutionenvironmentFormValue.NameRU,
        NameKK: pollutionenvironmentFormValue.NameKK,
        Name: ''
      }
      this.service.post(pollutionenvironment)
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
