import { Component, OnInit, LOCALE_ID, Inject } from '@angular/core';

import { MatSnackBar } from '@angular/material/snack-bar';

import { TranslateService } from '@ngx-translate/core';

import { UserService } from './user.service';
import { RegisterInfoComponent } from './registerinfo.component';

@Component({
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

  constructor(public userService: UserService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    private info: MatSnackBar) {
    translate.setDefaultLang(locale);
    translate.use(locale);
  }

  ngOnInit() {
    this.userService.formRegisterModel.reset();
  }

  register() {
    this.userService.register().subscribe(
      (res: any) => {
        if (res.Succeeded) {
          this.userService.formRegisterModel.reset();
          this.info.openFromComponent(RegisterInfoComponent, {
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
