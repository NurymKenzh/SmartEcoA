import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { TesterService } from './tester.service';
import { Tester } from './tester.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class TesterEditComponent implements OnInit {
  public testerForm: FormGroup;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: TesterService) { }

  ngOnInit() {
    this.testerForm = new FormGroup({
      Id: new FormControl(),
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.testerForm.patchValue(res as Tester);
      },
        (error => {
          console.log(error);
        })
    );
  }

  public error(control: string,
    error: string) {
    return this.testerForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/testers');
  }

  public save(testerFormValue) {
    if (this.testerForm.valid) {
      const tester: Tester = {
        Id: testerFormValue.Id,
        Name: testerFormValue.Name,
      }
      this.service.put(tester)
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
