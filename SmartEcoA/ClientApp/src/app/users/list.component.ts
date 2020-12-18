import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';

import { UserService } from './user.service';
import { User } from './user.model';

import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'users',
  templateUrl: 'list.component.html'
})

export class UsersListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['Email', 'Roles', 'details-edit-delete'];
  dataSource = new MatTableDataSource<User>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private userService: UserService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string) {
    this.dataSource.filterPredicate = (user: User, filter: string) => {
      return user.Email.toLowerCase().includes(filter.toLowerCase()) || user.Roles.join(' ').toLowerCase().includes(filter.toLowerCase());
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
    this.userService.get()
      .subscribe(res => {
        this.dataSource.data = res as User[];
      })
  }

  public filter(filter: string) {
    this.dataSource.filter = filter.trim().toLocaleLowerCase();
  }

  delete(Id) {
    if (confirm(this.translate.instant('AreYouSureDeleteThisRecord'))) {
      this.userService.delete(Id)
        .subscribe(() => {
          this.get();
        },
          err => {
            console.log(err);
          })
    }
  }
}
