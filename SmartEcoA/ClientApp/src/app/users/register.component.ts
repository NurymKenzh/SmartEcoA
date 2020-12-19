import { Component, OnInit, LOCALE_ID, Inject } from '@angular/core';
import { UserService } from './user.service';

import { TranslateService } from '@ngx-translate/core';

@Component({
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

  constructor(public userService: UserService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string) {
    translate.setDefaultLang(locale);
    translate.use(locale);
  }

  ngOnInit() {
    this.userService.formRegisterModel.reset();
  }

  register() {
    this.userService.register().subscribe(
      (res: any) => {
        if (res.succeeded) {
          this.userService.formRegisterModel.reset();
          alert(this.translate.instant('Users.NewUserRegistered'));
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
