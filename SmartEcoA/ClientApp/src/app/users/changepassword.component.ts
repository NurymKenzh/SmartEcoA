import { Component, OnInit, LOCALE_ID, Inject } from '@angular/core';

import { MatSnackBar } from '@angular/material/snack-bar';

import { TranslateService } from '@ngx-translate/core';

import { UserService } from './user.service';
import { ChangePasswordInfoComponent } from './changepasswordinfo.component';

@Component({
  templateUrl: './changepassword.component.html'
})
export class ChangePasswordComponent implements OnInit {

  constructor(public userService: UserService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    private info: MatSnackBar) {
    translate.setDefaultLang(locale);
    translate.use(locale);
  }

  ngOnInit() {
    this.userService.formChangePasswordModel.reset();
  }

  changePassword() {
    this.userService.changePassword().subscribe(
      (res: any) => {
        if (res.Succeeded) {
          this.userService.formChangePasswordModel.reset();
          this.info.openFromComponent(ChangePasswordInfoComponent, {
            duration: 5 * 1000,
          });
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
