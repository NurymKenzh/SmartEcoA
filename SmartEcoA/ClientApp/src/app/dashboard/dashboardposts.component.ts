import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';

import { MatOption, MatSelect } from "@angular/material";

import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';

import { Post } from '../posts/post.model';
import { PostService } from '../posts/post.service';

import { MeasuredParameterService } from '../measuredparameters/measuredparameter.service';
import { MeasuredParameter } from '../measuredparameters/measuredparameter.model';

import { PostDataAvgService } from '../postdataavgs/postdataavg.service';
import { PostDataAvg } from '../postdataavgs/postdataavg.model';

import { OLService } from '../ol/ol.service';
import Map from 'ol/Map';
import VectorSource from 'ol/source/Vector';
import VectorLayer from 'ol/layer/Vector';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import * as olProj from 'ol/proj';
import { Icon, Style, Text, Fill, Stroke } from 'ol/style';

import * as d3 from "d3";

@Component({
  selector: 'dashboardposts',
  templateUrl: './dashboardposts.component.html',
  styleUrls: ['./dashboardposts.component.css']
})
export class DashboardPostsComponent implements AfterViewInit {
  map: Map;
  background = new FormControl('OSM');
  Layer_posts = new VectorLayer();

  posts: Post[];
  public allPosts;
  public selectedPosts = [];
  public postsId = [];

  MeasuredParameterId = new FormControl();
  measuredparameters: MeasuredParameter[];

  postdataavgs: PostDataAvg[];

  date = new FormControl(new Date());

  spinner = false;
  firstGetPostDataAvgs = true;

  @ViewChild('multiselect', { static: true }) multiselect: MatSelect;

  constructor(private olservice: OLService,
    private postService: PostService,
    private measuredParameterService: MeasuredParameterService,
    private postDataAvgService: PostDataAvgService) { }

  ngOnInit() {
    this.postService.get()
      .subscribe(res => {
        this.allPosts = res as Post;
        this.allPosts.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        this.allPosts.forEach(post => this.selectedPosts.push({ 'Id': post.Id, 'Name': post.Name, 'Selected': false }));
      })
    this.measuredParameterService.get()
      .subscribe(res => {
        this.measuredparameters = (res as MeasuredParameter[]).filter(function (element) {
          return (element.OceanusCode != null && element.OceanusCode != '');
        });
        this.measuredparameters.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
        if (this.measuredparameters.length > 0) {
          this.MeasuredParameterId.setValue(this.measuredparameters[0].Id);
        }
      });
    this.getPostDataAvgs();
  }

  ngAfterViewInit() {
    
  }

  afterGetPostDataAvgs() {
    if (this.firstGetPostDataAvgs) {
      this.firstGetPostDataAvgs = false;
    }
    else {
      return;
    }
    this.olservice.olmap();

    var Source_posts = new VectorSource({});
    this.Layer_posts = new VectorLayer({
      source: Source_posts,
      style: this.postStyleFunction,
    });
    this.olservice.map.addLayer(this.Layer_posts);

    this.olservice.map.on("singleclick", function (evt) {
      evt.map.forEachFeatureAtPixel(evt.pixel, function (feature, layer) {
        var id = feature.get('id');
        this.multiselect.options.forEach((item: MatOption) => {
          if (item.value == id) {
            if (!item.selected) {
              this.selectPost(item, feature)
            } else {
              this.deselectPost(item, feature)
            }
          }
        });
      }.bind(this));
    }.bind(this));

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

    this.postService.get().subscribe(res => {
      this.posts = res as Post[];
      this.posts.forEach(function (value) {
        var polyFeature = new Feature({
          geometry: new Point(olProj.transform([value.Longitude, value.Latitude], 'EPSG:4326', 'EPSG:3857')),
          id: value.Id,
          Name: value.Name
        });

        Source_posts.addFeature(polyFeature);
      });
      this.olservice.map.getView().fit(Source_posts.getExtent());
    });
  }

  changeBackground(background) {
    this.olservice.changeBackground(background);
  }

  createPostTextStyle(feature) {
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

  public postStyleFunction = (feature) => {
    return new Style({
      image: new Icon({
        //color: 'rgba(0, 255, 255, 1)',
        crossOrigin: 'anonymous',
        src: '../images/icons/outline_circle_black_24dp.png'
      }),
      text: this.createPostTextStyle(feature),
    });
  }

  public selectedPostStyleFunction = (feature) => {
  return new Style({
    image: new Icon({
      //color: 'rgba(0, 255, 255, 1)',
      crossOrigin: 'anonymous',
      src: '../images/icons/outline_check_circle_black_24dp.png',
    }),
    text: this.createPostTextStyle(feature),
  });
  }

  public selectPost(item, feature) {
    feature.setStyle(this.selectedPostStyleFunction(feature));
    var size = '12px';
    var height = 1;
    var weight = 'bold';
    var font = weight + ' ' + size + '/' + height + ' ' + 'Arial';
    var text = feature.getStyle().getText();
    text.setFont(font);
    feature.getStyle().setText(text);
    if (item != null) {
      item.select();
    }
  }

  public deselectPost(item, feature) {
    feature.setStyle(this.postStyleFunction(feature));
    item.deselect();
  }

  changePosts(posts) {
    var source = this.Layer_posts.getSource();
    var features = source.getFeatures();
    this.selectPostsOnMap(features, posts);
    this.postsId = posts.value;
    this.changePostDataAvgs();
  }

  selectPostsOnMap(features, carPosts) {
    features.forEach(feature => {
      if (carPosts.value.find(x => x == feature.get('id')) != undefined) {
        this.selectPost(null, feature);
      }
      else {
        feature.setStyle(this.postStyleFunction(feature));
      }
    })
  }

  changeDate() {
    this.getPostDataAvgs();
  }

  public getPostDataAvgs() {
    this.spinner = true;
    this.postDataAvgService.get(null, this.date.value, null, null)
      .subscribe(res => {
        this.postdataavgs = res as PostDataAvg[];
        this.spinner = false;
        this.afterGetPostDataAvgs();
      });
    this.changePostDataAvgs();
  }

  changePostDataAvgs() {
    //var chart = d3.select("#chart")
    //  .append("svg");

    //chart.text("The Graph")
    //  .select("#graph");

    //var nodes = [{ x: 30, y: 50 },
    //  { x: 50, y: 80 },
    //  { x: 90, y: 120 },
    //  { x: 100, y: 25 }];

    //var links = [
    //  { source: nodes[0], target: nodes[1] },
    //  { source: nodes[1], target: nodes[2] },
    //  { source: nodes[2], target: nodes[3] }
    //];

    //chart.selectAll(".line")
    //  .data(links)
    //  .enter()
    //  .append("line")
    //  .attr("x1", function (d) { return d.source.x })
    //  .attr("y1", function (d) { return d.source.y })
    //  .attr("x2", function (d) { return d.target.x })
    //  .attr("y2", function (d) { return d.target.y })
    //  .style("stroke", "rgb(6,120,155)");


    //-----------------------------------------------------------
    var bodySelection = d3.select("#chart");

    var svgSelection = bodySelection.append("svg")
      .attr("width", 50)
      .attr("height", 50);

    var circleSelection = svgSelection.append("circle")
      .attr("cx", 25)
      .attr("cy", 25)
      .attr("r", 25)
      .style("fill", "purple");

    var theData = [1, 2, 3];

    var p = d3.select("#chart").selectAll("p")
      .data(theData)
      .enter()
      .append("p")
      .text("hello");

    //-----------------------------------------------------------
  }
}
