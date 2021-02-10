import { Component, OnInit, ViewChild, AfterViewInit, LOCALE_ID, Inject } from '@angular/core';
import { FormControl } from '@angular/forms';
import { animate, state, style, transition, trigger } from '@angular/animations';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';

import { TranslateService } from '@ngx-translate/core';

import { PostDataAvgService } from './postdataavg.service';
import { PostDataAvg } from './postdataavg.model';

import { PostService } from '../posts/post.service';
import { Post } from '../posts/post.model';

import { MeasuredParameterService } from '../measuredparameters/measuredparameter.service';
import { MeasuredParameter } from '../measuredparameters/measuredparameter.model';

@Component({
  selector: 'postdataavgs',
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

export class PostDataAvgsListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['DateTime', 'Post', 'MeasuredParameter', 'Value', 'details'];
  dataSource = new MatTableDataSource<PostDataAvg>();
  Date = new FormControl(new Date());
  PostId = new FormControl();
  MeasuredParameterId = new FormControl();
  spinner = false;
  posts: Post[];
  measuredparameters: MeasuredParameter[];

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private service: PostDataAvgService,
    private translate: TranslateService,
    @Inject(LOCALE_ID) protected locale: string,
    public deleteDialog: MatDialog,
    private postService: PostService,
    private measuredParameterService: MeasuredParameterService) {
    this.dataSource.filterPredicate = (data: PostDataAvg, filter: string) => {
      return data.DateTime.toString().includes(filter)
        || (data.MeasuredParameter.KazhydrometCode ? data.MeasuredParameter.KazhydrometCode.toLowerCase().includes(filter) : false)
        || (data.MeasuredParameter.Name ? data.MeasuredParameter.Name.toLowerCase().includes(filter) : false)
        || (data.MeasuredParameter.OceanusCode ? data.MeasuredParameter.OceanusCode.toLowerCase().includes(filter) : false)
        || (data.Post.Information ? data.Post.Information.toLowerCase().includes(filter) : false)
        || (data.Post.MN ? data.Post.MN.toLowerCase().includes(filter) : false)
        || (data.Post.Name ? data.Post.Name.toLowerCase().includes(filter) : false)
        || (data.Post.PhoneNumber ? data.Post.PhoneNumber.toLowerCase().includes(filter) : false);
    };
    translate.setDefaultLang(locale);
    translate.use(locale);
  }

  ngOnInit() {
    this.postService.get()
      .subscribe(res => {
        this.posts = res as Post[];
        this.posts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      });
    this.measuredParameterService.get()
      .subscribe(res => {
        this.measuredparameters = res as MeasuredParameter[];
        this.measuredparameters.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      });
    this.get();
  }

  ngAfterViewInit() {
    this.dataSource.sortingDataAccessor = (item, property) => {
      switch (property) {
        case 'Post': return item.Post.Name;
        case 'MeasuredParameter': return item.MeasuredParameter.Name;
        default: return item[property];
      }
    };
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  public get() {
    this.spinner = true;
    this.service.get(null, this.Date.value, this.PostId.value, this.MeasuredParameterId.value)
      .subscribe(res => {
        this.dataSource.data = res as PostDataAvg[];
        this.spinner = false;
      })
  }

  public filter(filter: string) {
    this.dataSource.filter = filter.trim().toLocaleLowerCase();
  }

  changeParameter() {
    this.get();
  }
}
