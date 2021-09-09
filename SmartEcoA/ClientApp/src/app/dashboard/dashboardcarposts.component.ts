import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';

import { MatOption, MatSelect } from "@angular/material";

import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';

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
  selector: 'dashboardcarposts',
  templateUrl: './dashboardcarposts.component.html',
  styleUrls: ['./dashboardcarposts.component.css']
})
export class DashboardCarPostsComponent implements AfterViewInit {
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
  columns: string[] = ['CarPostName', 'EngineFuel', 'AmountMeasurements', 'AmountExceedances'];
  dataCarPosts = new MatTableDataSource<Report>();
  Layer_select_pasturepol = new VectorLayer();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('multiselect', { static: true }) multiselect: MatSelect;

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
    this.dataCarPosts.sort = this.sort;

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

      function styleSelectPostFunction(feature) {
        return new Style({
          image: new Icon({
            anchor: [0.5, 0.5],
            anchorXUnits: 'fraction',
            anchorYUnits: 'fraction',
            src: '../images/icons/outline_emoji_transportation_black_24dp_select.png',
            scale: 1.2,
          }),
          text: createCarPostTextStyle(feature),
        });
    }

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

    var Source_select_pasturepol = new VectorSource({});
    this.Layer_select_pasturepol = new VectorLayer({
      source: Source_select_pasturepol,
      style: carPostStyleFunction,
    });
    this.olservice.map.addLayer(this.Layer_select_pasturepol);

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

    this.olservice.map.on("singleclick", function (evt) {
      evt.map.forEachFeatureAtPixel(evt.pixel, function (feature, layer) {
        var id = feature.get('id');
        this.multiselect.options.forEach((item: MatOption) => {
          if (item.value == id) {
            if (!item.selected) {
              selectCarPost(item, feature)
            } else {
              deselectCarPost(item, feature)
            }
          }
        });
      }.bind(this));
    }.bind(this));

    function selectCarPost(item, feature) {
      feature.setStyle(styleSelectPostFunction(feature));
      feature.getStyle().getText().getFill().setColor('#2914e7');
      item.select();
    }

    function deselectCarPost(item, feature) {
      feature.setStyle(carPostStyleFunction(feature));
      item.deselect();
    }

    this.olservice.map.on("pointermove", function (evt) {
      var hit = this.forEachFeatureAtPixel(evt.pixel, function (feature, layer) {
        return true;
      });
      if (hit) {
        this.getTargetElement().style.cursor = 'pointer';
      } else {
        this.getTargetElement().style.cursor = '';
      }
    });

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
        this.olservice.map.getView().fit(Source_select_pasturepol.getExtent());
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
    var source = this.Layer_select_pasturepol.getSource();
    var features = source.getFeatures();
    this.selectCarPostsOnMap(features, carPosts);
    this.carPostsId = carPosts.value;
    this.get();
  }

  selectCarPostsOnMap(features, carPosts) {
    features.forEach(feature => {
      if (carPosts.value.find(x => x == feature.get('id')) != undefined) {
        feature.setStyle(this.styleSelectPostFunction(feature));
        feature.getStyle().getText().getFill().setColor('#2914e7');
      }
      else {
        feature.setStyle(this.carPostStyleFunction(feature));
      }
    })
  }


  createCarPostTextStyle(feature) {
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

  styleSelectPostFunction(feature) {
    return new Style({
      image: new Icon({
        anchor: [0.5, 0.5],
        anchorXUnits: 'fraction',
        anchorYUnits: 'fraction',
        src: '../images/icons/outline_emoji_transportation_black_24dp_select.png',
        scale: 1.2,
      }),
      text: this.createCarPostTextStyle(feature),
    });
  }

  carPostStyleFunction(feature) {
    return new Style({
      image: new Icon({
        anchor: [0.5, 0.5],
        anchorXUnits: 'fraction',
        anchorYUnits: 'fraction',
        src: '../images/icons/outline_emoji_transportation_black_24dp.png',
      }),
      text: this.createCarPostTextStyle(feature),
    });
  }

  //public CarPostSelectedOnList() {
  //  this.allCarPosts.forEach(carPost => this.selectedCarPosts.values.push({ 'Id': carPost.Id, 'Name': carPost.Name, 'Selected': false })
  //  if (this.allCarPosts) {

  //  }
  //}
}
