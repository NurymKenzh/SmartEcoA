import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';

import { MatOption, MatSelect } from "@angular/material";

import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';

import { Report } from './report.model';

import { Post } from '../posts/post.model';
import { PostService } from '../posts/post.service';

import { OLService } from '../ol/ol.service';
import Map from 'ol/Map';
import VectorSource from 'ol/source/Vector';
import VectorLayer from 'ol/layer/Vector';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import * as olProj from 'ol/proj';
import { Icon, Style, Text, Fill, Stroke } from 'ol/style';

@Component({
  selector: 'dashboardposts',
  templateUrl: './dashboardposts.component.html',
  styleUrls: ['./dashboardposts.component.css']
})
export class DashboardPostsComponent implements AfterViewInit {
  map: Map;
  posts: Post[];
  public allPosts;
  background = new FormControl('OSM');
  Layer_posts = new VectorLayer();

  constructor(private olservice: OLService,
    private postservice: PostService) { }

  ngOnInit() {
    this.postservice.get()
      .subscribe(res => {
        this.allPosts = res as Post;
        })
  }

  ngAfterViewInit() {
    this.olservice.olmap();

    function postStyleFunction(feature) {
      return new Style({
        image: new Icon({
          color: 'rgba(255, 0, 0, .5)',
          crossOrigin: 'anonymous',
          src: '../images/icons/bigdot.png',
          scale: 0.2,
        }),
        //text: createCarPostTextStyle(feature),
      });
    }

    var Source_posts = new VectorSource({});
    this.Layer_posts = new VectorLayer({
      source: Source_posts,
      style: postStyleFunction,
    });
    this.olservice.map.addLayer(this.Layer_posts);

    this.postservice.get()
      .subscribe(res => {
        this.posts = res as Post[];
        this.posts.forEach(function (value) {
          var polyFeature = new Feature({
            geometry: new Point(olProj.transform([value.Longitude, value.Latitude], 'EPSG:4326', 'EPSG:3857')),
            id: value.Id,
            Name: value.Name
          });

          Source_posts.addFeature(polyFeature);
          //polyFeature.setStyle(
          //  new Style({
          //    image: new Icon({
          //      color: 'rgba(255, 0, 0, .5)',
          //      crossOrigin: 'anonymous',
          //      src: '../images/icons/bigdot.png',
          //      scale: 0.2,
          //    }),
          //  })
          //);
        });
        this.olservice.map.getView().fit(Source_posts.getExtent());
      });
  }

  changeBackground(background) {
    this.olservice.changeBackground(background);
  }
}
