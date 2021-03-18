import { Component, AfterViewInit } from '@angular/core';
import { FormControl } from '@angular/forms';

import { CarPost } from '../carposts/carpost.model';
import { CarPostService } from '../carposts/carpost.service';

import { CarPostAnalytic } from '../carpostanalytics/carpostanalytic.model';
import { CarPostAnalyticService } from '../carpostanalytics/carpostanalytic.service';

import { OLService } from '../ol/ol.service';
import Map from 'ol/Map';
import VectorSource from 'ol/source/Vector';
import VectorLayer from 'ol/layer/Vector';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import * as olProj from 'ol/proj';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements AfterViewInit {
  map: Map;
  background = new FormControl('OSM');
  carposts: CarPost[];
  carpostanalytic = new CarPostAnalytic();

  constructor(private olservice: OLService,
    private carpostservice: CarPostService,
    public carpostanalyticservice: CarPostAnalyticService) { }

  ngAfterViewInit() {
    this.olservice.olmap();

    this.olservice.map
    var Source_select_pasturepol = new VectorSource({});
    var Layer_select_pasturepol = new VectorLayer({
      source: Source_select_pasturepol
    });
    this.olservice.map.addLayer(Layer_select_pasturepol);

    this.olservice.map.on("singleclick", function (evt) {
      this.carpostanalytic = new CarPostAnalytic();
      evt.map.forEachFeatureAtPixel(evt.pixel, function (feature, layer) {
        this.carpostanalytic.CarPost = new CarPost();
        this.carpostanalytic.CarPost.Name = feature.get('name');
        this.carpostanalyticservice.get(null, feature.get('id'), new Date().toLocaleString())
          .subscribe(res => {
            if (res.length) {
              this.carpostanalytic = res[0];
            }
          },
            (error => {
              console.log(error);
            })
          );
      }.bind(this));
    }.bind(this));

    this.carpostservice.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.forEach(function (value) {
          var polyFeature = new Feature({
            geometry: new Point(olProj.transform([value.Longitude, value.Latitude], 'EPSG:4326', 'EPSG:3857')),
            id: value.Id,
            name: value.Name
          });
          Source_select_pasturepol.addFeature(polyFeature);
        });
      });
  }

  changeBackground(background) {
    this.olservice.changeBackground(background);
  }
}
