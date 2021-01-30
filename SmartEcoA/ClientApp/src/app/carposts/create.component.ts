import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

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
  templateUrl: 'create.component.html',
  styleUrls: ['create.component.css']
})

export class CarPostCreateComponent implements OnInit {
  public carpostForm: FormGroup;
  map: Map;
  background = new FormControl('OSM');
  source = new VectorSource();

  constructor(private router: Router,
    private service: CarPostService,
    private olservice: OLService) { }

  ngOnInit() {
    this.carpostForm = new FormGroup({
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      Latitude: new FormControl('', [Validators.required]),
      Longitude: new FormControl('', [Validators.required]),
    });
    this.olservice.olmap();
    var layer = new VectorLayer({
      source: this.source
    });
    this.olservice.map.addLayer(layer);
    
    let component = this;
    this.olservice.map.on('click', function (event) {
      var coordinates = event.coordinate;
      component.ChangeCarPostCoordinates(coordinates[0], coordinates[1]);
      component.carpostForm.controls.Latitude.setValue(olProj.transform(coordinates, 'EPSG:3857', 'EPSG:4326')[1]);
      component.carpostForm.controls.Longitude.setValue(olProj.transform(coordinates, 'EPSG:3857', 'EPSG:4326')[0]);
    });
  }

  public error(control: string,
    error: string) {
    return this.carpostForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/carposts');
  }

  public create(carpostFormValue) {
    if (this.carpostForm.valid) {
      const carpost: CarPost = {
        Id: 0,
        Name: carpostFormValue.Name,
        Latitude: carpostFormValue.Latitude,
        Longitude: carpostFormValue.Longitude,
      }
      this.service.post(carpost)
        .subscribe(() => {
          this.router.navigateByUrl('/carposts');
        },
          (error => {
            console.log(error);
          })
        )
    }
  }

  changeBackground(background) {
    this.olservice.changeBackground(background);
  }

  public ChangeCoordinates() {
    if (!isNaN(parseFloat(this.carpostForm.controls.Longitude.value)) && !isNaN(this.carpostForm.controls.Latitude.value)) {
      this.ChangeCarPostCoordinates(
        olProj.transform([this.carpostForm.controls.Longitude.value, this.carpostForm.controls.Latitude.value], 'EPSG:4326', 'EPSG:3857')[0],
        olProj.transform([this.carpostForm.controls.Longitude.value, this.carpostForm.controls.Latitude.value], 'EPSG:4326', 'EPSG:3857')[1]
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
