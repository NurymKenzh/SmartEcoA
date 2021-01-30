import { Component, AfterViewInit } from '@angular/core';
import { FormControl } from '@angular/forms';

import { OLService } from '../ol/ol.service';
import Map from 'ol/Map';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements AfterViewInit {
  map: Map;
  background = new FormControl('OSM');

  constructor(private olservice: OLService) { }

  ngAfterViewInit() {
    this.olservice.olmap();
  }

  changeBackground(background) {
    this.olservice.changeBackground(background);
  }
}
