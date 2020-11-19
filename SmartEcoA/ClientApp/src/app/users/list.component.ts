import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';

import { UserService } from './user.service';
import { User } from './user.model';

@Component({
  selector: 'users',
  templateUrl: 'list.component.html'
})

export class UsersListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['Email', 'Roles', 'details-edit-delete'];
  dataSource = new MatTableDataSource<User>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private userService: UserService) {
    this.dataSource.filterPredicate = (user: User, filter: string) => {
      return user.Email.toLowerCase().includes(filter.toLowerCase()) || user.Roles.join(' ').toLowerCase().includes(filter.toLowerCase());
    };
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
    if (confirm('Are you sure to delete this record ?')) {
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
