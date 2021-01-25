import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';
import { FormControl } from '@angular/forms';
import { animate, state, style, transition, trigger } from '@angular/animations';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

import { PostDataDividedService } from './postdatadivided.service';
import { PostDataDivided } from './postdatadivided.model';

import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'postdatadivideds',
  templateUrl: 'list.component.html',
  styleUrls: ['list.component.css'],
  animations: [
    trigger('expandTrigger', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*', 'border-bottom-style': 'solid' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ]
})

export class PostDataDividedsListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['MN', 'OceanusCode', 'Value', 'details'];
  dataSource = new MatTableDataSource<PostDataDivided>();
  date = new FormControl(new Date());
  spinner = false;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private service: PostDataDividedService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    public deleteDialog: MatDialog) {
    this.dataSource.filterPredicate = (data: PostDataDivided, filter: string) => {
      return data.MN.toLowerCase().includes(filter)
        || data.OceanusCode.toLowerCase().includes(filter)
        || data.Value.toString().includes(filter);
    };
    translate.setDefaultLang(locale);
    translate.use(locale);
  }

  ngOnInit() {
    this.get();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  public get() {
    this.spinner = true;
    this.service.get(null, this.date.value)
      .subscribe(res => {
        this.dataSource.data = res as PostDataDivided[];
        this.spinner = false;
      })
  }

  public filter(filter: string) {
    this.dataSource.filter = filter.trim().toLocaleLowerCase();
  }

  changeDate() {
    this.get();
  }
}
