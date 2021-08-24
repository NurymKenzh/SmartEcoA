import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { TypeEcoClassService } from './typeecoclass.service';
import { TypeEcoClass } from './typeecoclass.model';

@Component({
  templateUrl: 'edit.component.html'
})

export class TypeEcoClassEditComponent implements OnInit {
  public typeecoclassForm: FormGroup;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: TypeEcoClassService) { }

  ngOnInit() {
    this.typeecoclassForm = new FormGroup({
      Id: new FormControl(),
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      EngineType: new FormControl(''),
      MIN_TAH: new FormControl(''),
      DEL_MIN: new FormControl(''),
      MAX_TAH: new FormControl(''),
      DEL_MAX: new FormControl(''),
      MIN_CO: new FormControl(''),
      MAX_CO: new FormControl(''),
      MIN_CH: new FormControl(''),
      MAX_CH: new FormControl(''),
      L_MIN: new FormControl(''),
      L_MAX: new FormControl(''),
      K_SVOB: new FormControl(''),
      K_MAX: new FormControl(''),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.typeecoclassForm.patchValue(res as TypeEcoClass);
      },
        (error => {
          console.log(error);
        })
    );
  }

  public error(control: string,
    error: string) {
    return this.typeecoclassForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/typeecoclasses');
  }

  public save(typeecoclassFormValue) {
    if (this.typeecoclassForm.valid) {
      const typeecoclass: TypeEcoClass = {
        Id: typeecoclassFormValue.Id,
        Name: typeecoclassFormValue.Name,
        EngineType: typeecoclassFormValue.EngineType,
        MIN_TAH: typeecoclassFormValue.MIN_TAH,
        DEL_MIN: typeecoclassFormValue.DEL_MIN,
        MAX_TAH: typeecoclassFormValue.MAX_TAH,
        DEL_MAX: typeecoclassFormValue.DEL_MAX,
        MIN_CO: typeecoclassFormValue.MIN_CO,
        MAX_CO: typeecoclassFormValue.MAX_CO,
        MIN_CH: typeecoclassFormValue.MIN_CH,
        MAX_CH: typeecoclassFormValue.MAX_CH,
        L_MIN: typeecoclassFormValue.L_MIN,
        L_MAX: typeecoclassFormValue.L_MAX,
        K_SVOB: typeecoclassFormValue.K_SVOB,
        K_MAX: typeecoclassFormValue.K_MAX,
      }
      this.service.put(typeecoclass)
        .subscribe(() => {
          this.router.navigateByUrl('/typeecoclasses');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
