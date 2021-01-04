import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

import { PollutionEnvironmentService } from './pollutionenvironment.service';
import { PollutionEnvironment } from './pollutionenvironment.model';
import { PollutionEnvironmentDeleteComponent } from './delete.component';

import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'pollutionenvironments',
  templateUrl: 'list.component.html'
})

export class PollutionEnvironmentsListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['Name', 'details-edit-delete'];
  dataSource = new MatTableDataSource<PollutionEnvironment>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private service: PollutionEnvironmentService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    public deleteDialog: MatDialog) {
    this.dataSource.filterPredicate = (data: PollutionEnvironment, filter: string) => {
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
        this.dataSource.data = res as PollutionEnvironment[];
      })
  }

  delete(Id) {
    const deleteDialog = this.deleteDialog.open(PollutionEnvironmentDeleteComponent);
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

  public filter(filter: string) {
    this.dataSource.filter = filter.trim().toLocaleLowerCase();
  }
}
