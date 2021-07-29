import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

import { CarPostDataAutoTestService } from './carpostdataautotest.service';
import { CarPostDataAutoTest } from './carpostdataautotest.model';
import { CarPostDataAutoTestDeleteComponent } from './delete.component';

import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'carpostdataautotests',
  templateUrl: 'list.component.html'
})

export class CarPostDataAutoTestsListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['DateTime', 'CarPost', 'CarModelAutoTest', 'Number', 'details-edit-delete'];
  dataSource = new MatTableDataSource<CarPostDataAutoTest>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private service: CarPostDataAutoTestService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    public deleteDialog: MatDialog) {
    this.dataSource.filterPredicate = (data: CarPostDataAutoTest, filter: string) => {
      return (data.CarModelAutoTest ? (data.CarModelAutoTest.CarPost ? data.CarModelAutoTest.CarPost.Name.toLowerCase().includes(filter) : true) : true)
        || data.CarModelAutoTest.Name.toLowerCase().includes(filter)
        || (data.Number ? data.Number.toLowerCase().includes(filter) : true);
    };
    translate.setDefaultLang(locale);
    translate.use(locale);
  }

  ngOnInit() {
    this.get();
  }

  ngAfterViewInit() {
    this.dataSource.sortingDataAccessor = (item, property) => {
      switch (property) {
        case 'CarModelAutoTest': return item.CarModelAutoTest.Name;
        case 'CarPost': return item.CarModelAutoTest.CarPost.Name;
        default: return item[property];
      }
    };
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  public get() {
    this.service.get()
      .subscribe(res => {
        this.dataSource.data = res as CarPostDataAutoTest[];
      })
  }

  delete(Id) {
    const deleteDialog = this.deleteDialog.open(CarPostDataAutoTestDeleteComponent);
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
