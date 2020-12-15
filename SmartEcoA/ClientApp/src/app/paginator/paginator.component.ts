import { Injectable, Inject, LOCALE_ID } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class Paginator extends MatPaginatorIntl {
  itemsPerPageLabel: string;

  constructor(translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string) {
    super();
    translate.setDefaultLang(locale);
    translate.use(locale);
    translate.get('Paginator.ItemsPerPage').subscribe((res: string) => {
      this.itemsPerPageLabel = res;
      this.getRangeLabel = (page: number, pageSize: number, length: number) => {
        if (length == 0 || pageSize == 0) { return `0 / ${length}`; }
        length = Math.max(length, 0);
        const startIndex = page * pageSize;
        const endIndex = startIndex < length ?
          Math.min(startIndex + pageSize, length) :
          startIndex + pageSize;
        return `${startIndex + 1} - ${endIndex} / ${length}`;
      }
    });
  }
}
