import { Component, OnInit, LOCALE_ID, Inject } from '@angular/core';

@Component({
  templateUrl: 'index.component.html'
})

export class LocaleIndexComponent implements OnInit {
  languages = [
    { code: 'en', name: 'English' },
    { code: 'ru', name: 'Русский' },
    { code: 'kk', name: 'Қазақ' }
  ];

  constructor(@Inject(LOCALE_ID) protected locale: string) { }

  ngOnInit() { }
}
