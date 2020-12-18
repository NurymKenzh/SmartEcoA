import { Component, LOCALE_ID, Inject } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  constructor(@Inject(LOCALE_ID) protected locale: string) {
    if (!(window.location.href.includes('/ru/') || window.location.href.includes('/en/') || window.location.href.includes('/kk/'))) {
      window.location.href = '/' + this.getUsersLocale('ru') + '/';
    }
  }

  getUsersLocale(defaultValue: string): string {
    if (typeof window === 'undefined' || typeof window.navigator === 'undefined') {
      return defaultValue;
    }
    const wn = window.navigator as any;
    let lang = wn.languages ? wn.languages[0] : defaultValue;
    lang = lang || wn.language || wn.browserLanguage || wn.userLanguage;
    return lang;
  }
}
