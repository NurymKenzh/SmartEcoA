import { Component, OnInit, LOCALE_ID, Inject } from '@angular/core';
import { UserService } from './user.service';

import { TranslateService } from '@ngx-translate/core';

@Component({
  templateUrl: './changepassword.component.html'
})
export class ChangePasswordComponent implements OnInit {

  constructor(public userService: UserService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string) {
    translate.setDefaultLang(locale);
    translate.use(locale);
  }

  ngOnInit() {
    this.userService.formChangePasswordModel.reset();
  }

  changePassword() {
    this.userService.changePassword().subscribe(
      (res: any) => {
        if (res.succeeded) {
          this.userService.formChangePasswordModel.reset();
          alert(this.translate.instant('Users.PasswordChanged'));
        } else {
          res.Errors.forEach(element => {
            console.log(element.Description);
          });
        }
      },
      error => {
        console.log(error);
      }
    );
  }
}
