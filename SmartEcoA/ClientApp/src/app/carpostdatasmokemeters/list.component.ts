import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

import { CarPostDataSmokeMeterService } from './carpostdatasmokemeter.service';
import { CarPostDataSmokeMeter } from './carpostdatasmokemeter.model';
import { CarPostDataSmokeMeterDeleteComponent } from './delete.component';

import { TranslateService } from '@ngx-translate/core';
import { FormControl } from '@angular/forms';
import { CarPost } from '../carposts/carpost.model';
import { CarPostService } from '../carposts/carpost.service';

@Component({
  selector: 'carpostdatasmokemeters',
  templateUrl: 'list.component.html',
  styleUrls: ['list.component.css']
})

export class CarPostDataSmokeMetersListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['DateTime', 'CarPost', 'CarModelSmokeMeter', 'Number', 'details-edit-delete'];
  dataSource = new MatTableDataSource<CarPostDataSmokeMeter>();
  Date = new FormControl(new Date());
  CarPostId = new FormControl();
  carposts: CarPost[];

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private service: CarPostDataSmokeMeterService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    public deleteDialog: MatDialog,
    private carpostService: CarPostService) {
    this.dataSource.filterPredicate = (data: CarPostDataSmokeMeter, filter: string) => {
      return data.CarModelSmokeMeter.CarPost.Name.toLowerCase().includes(filter)
        || data.CarModelSmokeMeter.Name.toLowerCase().includes(filter)
        || data.Number.toLowerCase().includes(filter);
    };
    translate.setDefaultLang(locale);
    translate.use(locale);
  }

  ngOnInit() {
    this.carpostService.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      });
    this.get();
  }

  ngAfterViewInit() {
    this.dataSource.sortingDataAccessor = (item, property) => {
      switch (property) {
        case 'CarModelSmokeMeter': return item.CarModelSmokeMeter.Name;
        case 'CarPost': return item.CarModelSmokeMeter.CarPost.Name;
        default: return item[property];
      }
    };
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  public get() {
    this.service.get(null, this.CarPostId.value, this.Date.value)
      .subscribe(res => {
        this.dataSource.data = res as CarPostDataSmokeMeter[];
      })
  }

  delete(Id) {
    const deleteDialog = this.deleteDialog.open(CarPostDataSmokeMeterDeleteComponent);
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

  changeParameter() {
    this.get();
  }
}
