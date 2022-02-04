import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { AppealCitizenService } from './appealcitizen.service';
import { Question } from './question.model';

@Component({
  templateUrl: 'create.component.html'
})

export class AppealCitizenCreateComponent implements OnInit {
  public questionForm: FormGroup;

  constructor(private router: Router,
    private service: AppealCitizenService) { }

  ngOnInit() {
    this.questionForm = new FormGroup({
      NameUser: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      Text: new FormControl('', [Validators.required]),
    });
  }

  public error(control: string,
    error: string) {
    return this.questionForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/appealcitizens');
  }

  public create(questionFormValue) {
    if (this.questionForm.valid) {
      const question: Question = {
        Id: 0,
        Name: questionFormValue.NameUser,
        Text: questionFormValue.Text,
        DateTime: new Date(),
        ApplicationUser: null
      }
      this.service.postQuestion(question)
        .subscribe(() => {
          this.router.navigateByUrl('/appealcitizens');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }
}
