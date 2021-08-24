import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { TesterService } from './tester.service';
import { Tester } from './tester.model';

@Component({
  templateUrl: 'create.component.html',
  styleUrls: ['create.component.css']
})

export class TesterCreateComponent implements OnInit {
  public testerForm: FormGroup;

  constructor(private router: Router,
    private service: TesterService) { }

  ngOnInit() {
    this.testerForm = new FormGroup({
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    });
  }

  public error(control: string,
    error: string) {
    return this.testerForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/testers');
  }

  public create(testerFormValue) {
    if (this.testerForm.valid) {
      const tester: Tester = {
        Id: 0,
        Name: testerFormValue.Name,
      }
      this.service.post(tester)
        .subscribe(() => {
          this.router.navigateByUrl('/testers');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
