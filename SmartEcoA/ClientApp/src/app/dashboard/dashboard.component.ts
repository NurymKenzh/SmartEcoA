import { Component, AfterViewInit } from '@angular/core';
import { FormControl } from '@angular/forms';

import { MatTableDataSource } from '@angular/material/table';

import { CarPost } from '../carposts/carpost.model';
import { CarPostService } from '../carposts/carpost.service';

import { CarPostAnalytic } from '../carpostanalytics/carpostanalytic.model';
import { CarPostAnalyticService } from '../carpostanalytics/carpostanalytic.service';

import { Report } from './report.model';

import { OLService } from '../ol/ol.service';
import Map from 'ol/Map';
import VectorSource from 'ol/source/Vector';
import VectorLayer from 'ol/layer/Vector';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import * as olProj from 'ol/proj';
import { Icon, Style, Text, Fill, Stroke } from 'ol/style';

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
  startDate = new FormControl(new Date());
  endDate = new FormControl(new Date());
  public allCarPosts;
  public selectedCarPosts = [];
  public carPostsId = [];
  spinner = false;
  columns: string[] = ['CarPostName', 'EngineFuel', 'AmountMeasurements', 'AmountExceedances', 'Version'];
  dataCarPosts = new MatTableDataSource<Report>();

  constructor(private olservice: OLService,
    private carpostservice: CarPostService,
    public carpostanalyticservice: CarPostAnalyticService) { }

  ngOnInit() {
    this.carpostservice.get()
      .subscribe(res => {
        this.allCarPosts = res as CarPost;
        this.allCarPosts.forEach(carPost => this.selectedCarPosts.push({ 'Id': carPost.Id, 'Name': carPost.Name, 'Selected': false }));
      })
  }

  ngAfterViewInit() {
    this.olservice.olmap();

    var createCarPostTextStyle = function (feature) {
      var align = 'center';
      var baseline = 'middle';
      var size = '12px';
      var height = 1;
      var offsetX = 0;
      var offsetY = -12;
      var weight = 'normal';
      var placement = 'point';
      var maxAngle = undefined;
      var overflow = false;
      var rotation = 0;
      var font = weight + ' ' + size + '/' + height + ' ' + 'Arial';
      var fillColor = '#000000';
      var outlineColor = '#ffffff';
      var outlineWidth = 3;

      return new Text({
        textAlign: align == '' ? undefined : align,
        textBaseline: baseline,
        font: font,
        text: feature.get('Name'),
        fill: new Fill({ color: fillColor }),
        stroke: new Stroke({ color: outlineColor, width: outlineWidth }),
        offsetX: offsetX,
        offsetY: offsetY,
        placement: placement,
        maxAngle: maxAngle,
        overflow: overflow,
        rotation: rotation,
      });
    };

    function carPostStyleFunction(feature) {
      return new Style({
        image: new Icon({
        anchor: [0.5, 0.5],
        anchorXUnits: 'fraction',
        anchorYUnits: 'fraction',
        src: '../images/icons/outline_emoji_transportation_black_24dp.png',
      }),
        text: createCarPostTextStyle(feature),
      });
    }

    this.olservice.map
    var Source_select_pasturepol = new VectorSource({});
    var Layer_select_pasturepol = new VectorLayer({
      source: Source_select_pasturepol,
      style: carPostStyleFunction,
    });
    this.olservice.map.addLayer(Layer_select_pasturepol);

    //this.olservice.map.on("singleclick", function (evt) {
    //  this.carpostanalytic = new CarPostAnalytic();
    //  evt.map.forEachFeatureAtPixel(evt.pixel, function (feature, layer) {
    //    this.carpostanalytic.CarPost = new CarPost();
    //    this.carpostanalytic.CarPost.Name = feature.get('name');
    //    this.carpostanalyticservice.get(null, feature.get('id'), new Date().toLocaleString())
    //      .subscribe(res => {
    //        if (res.length) {
    //          this.carpostanalytic = res[0];
    //        }
    //      },
    //        (error => {
    //          console.log(error);
    //        })
    //      );
    //  }.bind(this));
    //}.bind(this));

    this.carpostservice.get()
      .subscribe(res => {
        this.carposts = res as CarPost[];
        this.carposts.forEach(function (value) {
          var polyFeature = new Feature({
            geometry: new Point(olProj.transform([value.Longitude, value.Latitude], 'EPSG:4326', 'EPSG:3857')),
            id: value.Id,
            Name: value.Name
          });

          Source_select_pasturepol.addFeature(polyFeature);
        });
      });
  }

  changeBackground(background) {
    this.olservice.changeBackground(background);
  }

  public get() {
    this.spinner = true;
    this.carpostservice.report(this.startDate.value, this.endDate.value, this.carPostsId)
      .subscribe(res => {
        this.dataCarPosts.data = res as Report[];
        this.spinner = false;
      })
  }

  changeDate() {
    this.get();
  }

  changeCarPosts(carPosts) {
    this.carPostsId = carPosts.value;
    this.get();
  }
}
