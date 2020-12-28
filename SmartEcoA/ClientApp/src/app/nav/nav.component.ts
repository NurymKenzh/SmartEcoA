import { Component, OnInit, LOCALE_ID, Inject } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { AuthorizedUser, UserService } from '../users/user.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent {
  authorizedUserInfo;
  authorizedUser: AuthorizedUser;

  languages = [
    { code: 'en', name: 'English' },
    { code: 'ru', name: 'Русский' },
    { code: 'kk', name: 'Қазақ' }
  ];
  currentLanguage: string;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  constructor(private breakpointObserver: BreakpointObserver,
    public userService: UserService,
    @Inject(LOCALE_ID) protected locale: string) {
    this.currentLanguage = (this.languages.filter(l => { return l.code === this.locale }).length > 0)
      ? (this.languages.filter(l => { return l.code === this.locale })[0].name)
      : (this.languages[0].name);
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
  }
}
