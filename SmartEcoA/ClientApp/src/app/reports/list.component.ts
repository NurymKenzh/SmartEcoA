import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

import { ReportService } from './report.service';
import { Report } from './report.model';
import { ReportDeleteComponent } from './delete.component';

import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'reports',
  templateUrl: 'list.component.html',
  styleUrls: ['list.component.css'],
})

export class ReportsListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['Name', 'Email', 'DateTime', 'InputParameters', 'FileName', 'details-delete'];
  dataSource = new MatTableDataSource<Report>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private service: ReportService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    public deleteDialog: MatDialog,
    private http: HttpClient  ) {
    this.dataSource.filterPredicate = (data: Report, filter: string) => {
      return data.Name.toLowerCase().includes(filter);
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
    this.service.get()
      .subscribe(res => {
        this.dataSource.data = res as Report[];
      })
  }

  delete(Id) {
    const deleteDialog = this.deleteDialog.open(ReportDeleteComponent);
    deleteDialog.afterClosed().subscribe(result => {
      if (result) {
        this.service.delete(Id)
          .subscribe(() => {
            this.get();
          },
          err => {
            console.log(err);
          })
      }
    });
  }

  public download(Id, FileName) {
    this.service.download(Id, FileName);
  }

  public filter(filter: string) {
    this.dataSource.filter = filter.trim().toLocaleLowerCase();
  }
}
