import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { AppealCitizenService } from './appealcitizen.service';
import { Answer } from './answer.model';
import { Question } from './question.model';

@Component({
  templateUrl: 'createanswer.component.html'
})

export class AppealCitizenCreateAnswerComponent implements OnInit {
  public answerForm: FormGroup;
  public idQuestion: number;
  public questionForAnswer: Question;

  constructor(private router: Router,
    private service: AppealCitizenService,
    private activatedRoute: ActivatedRoute ) { }

  ngOnInit() {
    this.answerForm = new FormGroup({
      Text: new FormControl('', [Validators.required]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.questionForAnswer = res as Question;
      },
        (error => {
          console.log(error);
        })
      );
  }

  public error(control: string,
    error: string) {
    return this.answerForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/appealcitizens');
  }

  public create(answerFormValue) {
    if (this.answerForm.valid) {
      const answer: Answer = {
        Id: 0,
        Text: answerFormValue.Text,
        DateTime: new Date(),
        ApplicationUser: null,
        Question: this.questionForAnswer,
      }
      this.service.postAnswer(answer)
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
