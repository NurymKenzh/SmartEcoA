import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl } from '@angular/forms';

import { PostService } from './post.service';
import { Post } from './post.model';

import { OLService } from '../ol/ol.service';
import Map from 'ol/Map';
import VectorSource from 'ol/source/Vector';
import VectorLayer from 'ol/layer/Vector';
import Point from 'ol/geom/Point';
import Feature from 'ol/Feature';
import * as olProj from 'ol/proj';

@Component({
  templateUrl: 'details.component.html',
  styleUrls: ['details.component.css']
})

export class PostDetailsComponent implements OnInit {
  public post: Post;
  map: Map;
  background = new FormControl('OSM');
  source = new VectorSource();

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: PostService,
    private olservice: OLService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.post = res as Post;
        this.ChangeCoordinates();
      },
        (error => {
          console.log(error);
        })
    );

    this.olservice.olmap();
    var layer = new VectorLayer({
      source: this.source
    });
    this.olservice.map.addLayer(layer);
  }

  public cancel() {
    this.router.navigateByUrl('/posts');
  }

  changeBackground(background) {
    this.olservice.changeBackground(background);
  }

  public ChangeCoordinates() {
    if (this.post.Longitude != null && this.post.Latitude != null) {
      this.ChangePostCoordinates(
        olProj.transform([this.post.Longitude, this.post.Latitude], 'EPSG:4326', 'EPSG:3857')[0],
        olProj.transform([this.post.Longitude, this.post.Latitude], 'EPSG:4326', 'EPSG:3857')[1]
      );
    }
  }

  private ChangePostCoordinates(longitude, latitude) {
    var point = new Point(
      [longitude, latitude]
    );
    var feature = new Feature({
      geometry: point
    });
    this.source.clear();
    this.source.addFeature(feature);
  }
}
