import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DataProviderService } from './dataprovider.service';
import { DataProvider } from './dataprovider.model';

@Component({
  templateUrl: 'create.component.html'
})

export class DataProviderCreateComponent implements OnInit {
  public dataproviderForm: FormGroup;

  constructor(private router: Router,
    private service: DataProviderService) { }

  ngOnInit() {
    this.dataproviderForm = new FormGroup({
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    });
  }

  public error(control: string,
    error: string) {
    return this.dataproviderForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/dataproviders');
  }

  public create(dataproviderFormValue) {
    if (this.dataproviderForm.valid) {
      const dataprovider: DataProvider = {
        Id: 0,
        Name: dataproviderFormValue.Name
      }
      this.service.post(dataprovider)
        .subscribe(() => {
          this.router.navigateByUrl('/dataproviders');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
