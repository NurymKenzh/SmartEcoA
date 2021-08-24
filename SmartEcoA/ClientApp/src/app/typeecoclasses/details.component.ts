import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { TypeEcoClassService } from './typeecoclass.service';
import { TypeEcoClass } from './typeecoclass.model';

@Component({
  templateUrl: 'details.component.html'
})

export class TypeEcoClassDetailsComponent implements OnInit {
  public typeecoclass: TypeEcoClass;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: TypeEcoClassService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.typeecoclass = res as TypeEcoClass;
      },
        (error => {
          console.log(error);
        })
    );
  }

  public cancel() {
    this.router.navigateByUrl('/typeecoclasses');
  }
}
