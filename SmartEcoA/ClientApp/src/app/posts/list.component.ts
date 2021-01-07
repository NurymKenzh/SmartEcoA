import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

import { PostService } from './post.service';
import { Post } from './post.model';
import { PostDeleteComponent } from './delete.component';

import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'posts',
  templateUrl: 'list.component.html'
})

export class PostsListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['Project', 'Name', 'MN', 'Information', 'DataProvider', 'details-edit-delete'];
  dataSource = new MatTableDataSource<Post>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private service: PostService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    public deleteDialog: MatDialog) {
    this.dataSource.filterPredicate = (data: Post, filter: string) => {
      return data.Project.Name.toLowerCase().includes(filter)
        || data.Name.toLowerCase().includes(filter)
        || data.MN.toLowerCase().includes(filter)
        || data.Information.toLowerCase().includes(filter)
        || data.DataProvider.Name.toLowerCase().includes(filter);
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
        case 'Project': return item.Project.Name;
        case 'DataProvider': return item.DataProvider.Name;
        default: return item[property];
      }
    };
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  public get() {
    this.service.get()
      .subscribe(res => {
        this.dataSource.data = res as Post[];
      })
  }

  delete(Id) {
    const deleteDialog = this.deleteDialog.open(PostDeleteComponent);
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
