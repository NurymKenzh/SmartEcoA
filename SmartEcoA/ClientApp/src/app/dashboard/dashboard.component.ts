import { Component, OnInit } from '@angular/core';

import { OLService } from '../ol/ol.service';
import Map from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  map: Map;

  constructor(private olservice: OLService) { }

  ngOnInit() {
    //this.olmap();
    this.olservice.olmap();
  }

  //private olmap() {
  //  this.map = new Map({
  //    target: 'map',
  //    layers: [
  //      new TileLayer({
  //        source: new OSM()
  //      })
  //    ],
  //    view: new View({
  //      center: [0, 0],
  //      zoom: 2
  //    })
  //  });
  //  setTimeout(() => { this.map.updateSize(); });
  //}
}
