import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

import { CarModelSmokeMeterService } from './carmodelsmokemeter.service';
import { CarModelSmokeMeter } from './carmodelsmokemeter.model';
import { CarModelSmokeMeterDeleteComponent } from './delete.component';

import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'carmodelsmokemeters',
  templateUrl: 'list.component.html'
})

export class CarModelSmokeMetersListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['Name', 'CarPost', 'details-edit-delete'];
  dataSource = new MatTableDataSource<CarModelSmokeMeter>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private service: CarModelSmokeMeterService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    public deleteDialog: MatDialog) {
    this.dataSource.filterPredicate = (data: CarModelSmokeMeter, filter: string) => {
      return data.CarPost.Name.toLowerCase().includes(filter)
        || data.Name.toLowerCase().includes(filter);
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
        case 'CarPost': return item.CarPost.Name;
        default: return item[property];
      }
    };
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  public get() {
    this.service.get()
      .subscribe(res => {
        this.dataSource.data = res as CarModelSmokeMeter[];
      })
  }

  delete(Id) {
    const deleteDialog = this.deleteDialog.open(CarModelSmokeMeterDeleteComponent);
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
