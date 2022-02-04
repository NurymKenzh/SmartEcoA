import { Component, OnInit, AfterViewInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { AppealCitizenService } from './appealcitizen.service';
import { QuestionAndAnswers } from './questionandanswers.model';

import { TranslateService } from '@ngx-translate/core';
import { AppealCitizenDeleteComponent } from './delete.component';
import { AuthorizedUser, UserService } from '../users/user.service';

@Component({
  selector: 'appealcitizens',
  templateUrl: 'list.component.html'
})

export class AppealCitizensListComponent implements OnInit, AfterViewInit {
  questionAndAnswers: QuestionAndAnswers[];
  authorizedUser: AuthorizedUser;

  constructor(private service: AppealCitizenService,
    private translate: TranslateService,
    public deleteDialog: MatDialog,
    public userService: UserService) {
  }

  ngOnInit() {
    this.userService.authorizedUser$.subscribe((authorizedUser: AuthorizedUser) => {
      this.authorizedUser = authorizedUser;
    });
    if (localStorage.getItem('token')) {
      this.authorizedUser = {
        Email: this.userService.getAuthorizedUserEmail()
      };
    }
    this.get();
  }

  ngAfterViewInit() {
  }

  public get() {
    this.service.get()
      .subscribe(res => {
        this.questionAndAnswers = res as QuestionAndAnswers[];
      })
  }

  public deleteQuestion(Id) {
    const deleteDialog = this.deleteDialog.open(AppealCitizenDeleteComponent);
    deleteDialog.afterClosed().subscribe(result => {
      if (result) {
        this.service.deleteQuestion(Id)
          .subscribe(() => {
            this.get();
          },
            err => {
              console.log(err);
            })
      }
    });
  }

  public deleteAnswer(Id) {
    const deleteDialog = this.deleteDialog.open(AppealCitizenDeleteComponent);
    deleteDialog.afterClosed().subscribe(result => {
      if (result) {
        this.service.deleteAnswer(Id)
          .subscribe(() => {
            this.get();
          },
            err => {
              console.log(err);
            })
      }
    });
  }
}
