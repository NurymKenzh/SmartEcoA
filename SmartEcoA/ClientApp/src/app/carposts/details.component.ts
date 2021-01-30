import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl } from '@angular/forms';

import { CarPostService } from './carpost.service';
import { CarPost } from './carpost.model';

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

export class CarPostDetailsComponent implements OnInit {
  public carpost: CarPost;
  map: Map;
  background = new FormControl('OSM');
  source = new VectorSource();

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarPostService,
    private olservice: OLService) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carpost = res as CarPost;
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
    this.router.navigateByUrl('/carposts');
  }

  changeBackground(background) {
    this.olservice.changeBackground(background);
  }

  public ChangeCoordinates() {
    if (this.carpost.Longitude != null && this.carpost.Latitude != null) {
      this.ChangeCarPostCoordinates(
        olProj.transform([this.carpost.Longitude, this.carpost.Latitude], 'EPSG:4326', 'EPSG:3857')[0],
        olProj.transform([this.carpost.Longitude, this.carpost.Latitude], 'EPSG:4326', 'EPSG:3857')[1]
      );
    }
  }

  private ChangeCarPostCoordinates(longitude, latitude) {
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
