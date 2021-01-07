import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { PostService } from './post.service';
import { Post } from './post.model';

import { ProjectService } from '../projects/project.service';
import { Project } from '../projects/project.model';

import { PollutionEnvironmentService } from '../pollutionenvironments/pollutionenvironment.service';
import { PollutionEnvironment } from '../pollutionenvironments/pollutionenvironment.model';

import { DataProviderService } from '../dataproviders/dataprovider.service';
import { DataProvider } from '../dataproviders/dataprovider.model';

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

export class PostCreateComponent implements OnInit {
  public postForm: FormGroup;
  projects: Project[];
  pollutionenvironments: PollutionEnvironment[];
  dataproviders: DataProvider[];
  map: Map;
  background = new FormControl('OSM');
  source = new VectorSource();

  constructor(private router: Router,
    private service: PostService,
    private projectService: ProjectService,
    private pollutionEnvironmentService: PollutionEnvironmentService,
    private dataProviderService: DataProviderService,
    private olservice: OLService) { }

  ngOnInit() {
    this.projectService.get()
      .subscribe(res => {
        this.projects = res as Project[];
        this.projects.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0)); 
      });
    this.pollutionEnvironmentService.get()
      .subscribe(res => {
        this.pollutionenvironments = res as PollutionEnvironment[];
        this.pollutionenvironments.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0)); 
      });
    this.dataProviderService.get()
      .subscribe(res => {
        this.dataproviders = res as DataProvider[];
        this.dataproviders.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0)); 
      });
    this.postForm = new FormGroup({
      Name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      MN: new FormControl(''),
      Latitude: new FormControl('', [Validators.required]),
      Longitude: new FormControl('', [Validators.required]),
      Information: new FormControl(''),
      PhoneNumber: new FormControl(''),
      ProjectId: new FormControl(''),
      PollutionEnvironmentId: new FormControl('', [Validators.required]),
      DataProviderId: new FormControl('', [Validators.required]),
      KazhydrometID: new FormControl(''),
      Automatic: new FormControl(false, [Validators.required]),
    });
    this.olservice.olmap();
    var layer = new VectorLayer({
      source: this.source
    });
    this.olservice.map.addLayer(layer);
    
    let component = this;
    this.olservice.map.on('click', function (event) {
      var coordinates = event.coordinate;
      component.ChangePostCoordinates(coordinates[0], coordinates[1]);
      component.postForm.controls.Latitude.setValue(olProj.transform(coordinates, 'EPSG:3857', 'EPSG:4326')[1]);
      component.postForm.controls.Longitude.setValue(olProj.transform(coordinates, 'EPSG:3857', 'EPSG:4326')[0]);
    });
  }

  public error(control: string,
    error: string) {
    return this.postForm.controls[control].hasError(error);
  }

  public cancel() {
    this.router.navigateByUrl('/posts');
  }

  public create(postFormValue) {
    if (this.postForm.valid) {
      const post: Post = {
        Id: 0,
        Name: postFormValue.Name,
        MN: postFormValue.MN,
        Latitude: postFormValue.Latitude,
        Longitude: postFormValue.Longitude,
        Information: postFormValue.Information,
        PhoneNumber: postFormValue.PhoneNumber,
        ProjectId: postFormValue.ProjectId,
        Project: null,
        PollutionEnvironmentId: postFormValue.PollutionEnvironmentId,
        PollutionEnvironment: null,
        DataProviderId: postFormValue.DataProviderId,
        DataProvider: null,
        KazhydrometID: postFormValue.KazhydrometID,
        Automatic: postFormValue.Automatic,
      }
      this.service.post(post)
        .subscribe(() => {
          this.router.navigateByUrl('/posts');
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
    if (!isNaN(parseFloat(this.postForm.controls.Longitude.value)) && !isNaN(this.postForm.controls.Latitude.value)) {
      this.ChangePostCoordinates(
        olProj.transform([this.postForm.controls.Longitude.value, this.postForm.controls.Latitude.value], 'EPSG:4326', 'EPSG:3857')[0],
        olProj.transform([this.postForm.controls.Longitude.value, this.postForm.controls.Latitude.value], 'EPSG:4326', 'EPSG:3857')[1]
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
