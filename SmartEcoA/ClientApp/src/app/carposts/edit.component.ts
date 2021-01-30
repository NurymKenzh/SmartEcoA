import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
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
  templateUrl: 'edit.component.html',
  styleUrls: ['edit.component.css']
})

export class CarPostEditComponent implements OnInit {
  public carpostForm: FormGroup;
  map: Map;
  background = new FormControl('OSM');
  source = new VectorSource();

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: CarPostService,
    private olservice: OLService) { }

  ngOnInit() {
    this.carpostForm = new FormGroup({
      Id: new FormControl(),
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      Latitude: new FormControl('', [Validators.required]),
      Longitude: new FormControl('', [Validators.required]),
    });
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.get(id)
      .subscribe(res => {
        this.carpostForm.patchValue(res as CarPost);
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

  public save(carpostFormValue) {
    if (this.carpostForm.valid) {
      const carpost: CarPost = {
        Id: carpostFormValue.Id,
        Name: carpostFormValue.Name,
        Latitude: carpostFormValue.Latitude,
        Longitude: carpostFormValue.Longitude,
      }
      this.service.put(carpost)
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
