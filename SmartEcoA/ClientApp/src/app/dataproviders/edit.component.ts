import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DataProviderService } from './dataprovider.service';
import { DataProvider } from './dataprovider.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class DataProviderEditComponent implements OnInit {
  public dataproviderForm: FormGroup;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: DataProviderService) { }

  ngOnInit() {
    this.dataproviderForm = new FormGroup({
      Id: new FormControl(),
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.dataproviderForm.patchValue(res as DataProvider);
      },
        (error => {
          console.log(error);
        })
      );
  }

  public error(control: string,
    error: string) {
    return this.dataproviderForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/dataproviders');
  }

  public save(dataproviderFormValue) {
    if (this.dataproviderForm.valid) {
      const dataprovider: DataProvider = {
        Id: dataproviderFormValue.Id,
        Name: dataproviderFormValue.Name
      }
      this.service.put(dataprovider)
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
