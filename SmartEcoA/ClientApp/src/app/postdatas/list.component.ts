import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';
import { FormControl } from '@angular/forms';
import { animate, state, style, transition, trigger } from '@angular/animations';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

import { PostDataService } from './postdata.service';
import { PostData } from './postdata.model';

import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'postdatas',
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

export class PostDatasListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['DateTime', 'IP', 'Data', 'details'];
  dataSource = new MatTableDataSource<PostData>();
  date = new FormControl(new Date());

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private service: PostDataService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    public deleteDialog: MatDialog) {
    this.dataSource.filterPredicate = (data: PostData, filter: string) => {
      return data.DateTime.toString().includes(filter)
        || data.IP.toLowerCase().includes(filter)
        || data.Data.toLowerCase().includes(filter);
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
    this.service.get(null, this.date.value)
      .subscribe(res => {
        this.dataSource.data = res as PostData[];
      })
  }

  public filter(filter: string) {
    this.dataSource.filter = filter.trim().toLocaleLowerCase();
  }

  changeDate() {
    this.get();
  }
}
