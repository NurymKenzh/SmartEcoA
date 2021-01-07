import Map from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import BingMaps from 'ol/source/BingMaps';
import XYZ from 'ol/source/XYZ';
import Stamen from 'ol/source/Stamen';
import * as olProj from 'ol/proj';

export class OLService {
  map: Map;
  HEREappId = 'SLdBasp4s1oq2oUbVGxy';
  HEREappCode = 'D9-h9iVhjgUB_9eUlFETXA';
  HEREurlTemplate = 'https://{1-4}.{base}.maps.cit.api.here.com' +
    '/{type}/2.1/maptile/newest/{scheme}/{z}/{x}/{y}/256/png' +
    '?app_id={app_id}&app_code={app_code}';
  HERElayerDescNormalDay = {
    base: 'base',
    type: 'maptile',
    scheme: 'normal.day',
    app_id: this.HEREappId,
    app_code: this.HEREappCode
  };
  HERElayerDescNormalDayTransit = {
    base: 'base',
    type: 'maptile',
    scheme: 'normal.day.transit',
    app_id: this.HEREappId,
    app_code: this.HEREappCode
  };
  HERElayerDescTerrainDay = {
    base: 'aerial',
    type: 'maptile',
    scheme: 'terrain.day',
    app_id: this.HEREappId,
    app_code: this.HEREappCode
  };
  HERElayerDescHybridDay = {
    base: 'aerial',
    type: 'maptile',
    scheme: 'hybrid.day',
    app_id: this.HEREappId,
    app_code: this.HEREappCode
  };
  backgrounds = {
    Source_OSM: new OSM(),
    Source_BingAerialWithLabels: new BingMaps({
      key: 'AvXWBeyEt0tMq0Cmi-TdlXA6qG2GDpuV1UcSbSqNXbfHcQ41GFYtjpu0gz8RS6-b',
      imagerySet: 'AerialWithLabels',
    }),
    Source_BingRoadStatic: new BingMaps({
      key: 'AvXWBeyEt0tMq0Cmi-TdlXA6qG2GDpuV1UcSbSqNXbfHcQ41GFYtjpu0gz8RS6-b',
      imagerySet: 'Road',
    }),
    Source_BingRoadDynamic: new BingMaps({
      key: 'AvXWBeyEt0tMq0Cmi-TdlXA6qG2GDpuV1UcSbSqNXbfHcQ41GFYtjpu0gz8RS6-b',
      imagerySet: 'RoadOnDemand',
    }),
    Source_HERENormalDay: new XYZ({
      url: this.HEREcreateUrl(this.HEREurlTemplate, this.HERElayerDescNormalDay),
      attributions: 'Map Tiles &copy; ' + new Date().getFullYear() + ' ' +
        '<a href="http://developer.here.com">HERE</a>'
    }),
    Source_HERENormalDayTransit: new XYZ({
      url: this.HEREcreateUrl(this.HEREurlTemplate, this.HERElayerDescNormalDayTransit),
      attributions: 'Map Tiles &copy; ' + new Date().getFullYear() + ' ' +
        '<a href="http://developer.here.com">HERE</a>'
    }),
    Source_HERETerrainDay: new XYZ({
      url: this.HEREcreateUrl(this.HEREurlTemplate, this.HERElayerDescTerrainDay),
      attributions: 'Map Tiles &copy; ' + new Date().getFullYear() + ' ' +
        '<a href="http://developer.here.com">HERE</a>'
    }),
    Source_HEREHybridDay: new XYZ({
      url: this.HEREcreateUrl(this.HEREurlTemplate, this.HERElayerDescHybridDay),
      attributions: 'Map Tiles &copy; ' + new Date().getFullYear() + ' ' +
        '<a href="http://developer.here.com">HERE</a>'
    }),
    Source_StamenWatercolor: new Stamen({
      layer: 'watercolor'
    }),
    Source_StamenTerrain: new Stamen({
      layer: 'terrain'
    }),
    Source_StamenToner: new Stamen({
      layer: 'toner'
    }),
    Source_ArcGIS: new XYZ({
      attributions: 'Tiles © <a href="https://services.arcgisonline.com/ArcGIS/' +
        'rest/services/World_Topo_Map/MapServer">ArcGIS</a>',
      url: 'https://server.arcgisonline.com/ArcGIS/rest/services/' +
        'World_Topo_Map/MapServer/tile/{z}/{y}/{x}'
    }),
    Source_ThunderforestOpenCycleMap: new XYZ({
      url: 'https://{a-c}.tile.thunderforest.com/cycle/{z}/{x}/{y}.png' +
        '?apikey=6746f4299ea3479aba8726b09f049c1b'
    }),
    Source_ThunderforestTransport: new XYZ({
      url: 'https://{a-c}.tile.thunderforest.com/transport/{z}/{x}/{y}.png' +
        '?apikey=6746f4299ea3479aba8726b09f049c1b'
    }),
    Source_ThunderforestLandscape: new XYZ({
      url: 'https://{a-c}.tile.thunderforest.com/landscape/{z}/{x}/{y}.png' +
        '?apikey=6746f4299ea3479aba8726b09f049c1b'
    }),
    Source_ThunderforestOutdoors: new XYZ({
      url: 'https://{a-c}.tile.thunderforest.com/outdoors/{z}/{x}/{y}.png' +
        '?apikey=6746f4299ea3479aba8726b09f049c1b'
    }),
    Source_ThunderforestTransportDark: new XYZ({
      url: 'https://{a-c}.tile.thunderforest.com/transport-dark/{z}/{x}/{y}.png' +
        '?apikey=6746f4299ea3479aba8726b09f049c1b'
    }),
    Source_ThunderforestSpinalMap: new XYZ({
      url: 'https://{a-c}.tile.thunderforest.com/spinal-map/{z}/{x}/{y}.png' +
        '?apikey=6746f4299ea3479aba8726b09f049c1b'
    }),
    Source_ThunderforestPioneer: new XYZ({
      url: 'https://{a-c}.tile.thunderforest.com/pioneer/{z}/{x}/{y}.png' +
        '?apikey=6746f4299ea3479aba8726b09f049c1b'
    }),
    Source_ThunderforestMobileAtlas: new XYZ({
      url: 'https://{a-c}.tile.thunderforest.com/mobile-atlas/{z}/{x}/{y}.png' +
        '?apikey=6746f4299ea3479aba8726b09f049c1b'
    }),
    Source_ThunderforestNeighbourhood: new XYZ({
      url: 'https://{a-c}.tile.thunderforest.com/neighbourhood/{z}/{x}/{y}.png' +
        '?apikey=6746f4299ea3479aba8726b09f049c1b'
    })
  };

  public olmap() {
    this.map = new Map({
      target: 'map',
      layers: [
        new TileLayer({
          source: new OSM(),
          'name': 'Base'
        })
      ],
      view: new View({
        center: [0, 0],
        zoom: 2
      })
    });
    setTimeout(() => { this.map.updateSize(); });
    this.map.getView().fit(olProj.fromLonLat([46.750277777777775, 40.59638888888889]).concat(olProj.fromLonLat([86.57583333333334, 55.41138888888889])), this.map.getSize());
  }

  public changeBackground(background) {
    let component = this;
    this.map.getLayers().forEach(function (layer) {
      if (layer.get('name') == 'Base') {
        if (background == 'OSM') {
          layer.setSource(component.backgrounds.Source_OSM);
        }
        else if (background == 'BingAerialWithLabels') {
          layer.setSource(component.backgrounds.Source_BingAerialWithLabels);
        }
        else if (background == 'BingRoadStatic') {
          layer.setSource(component.backgrounds.Source_BingRoadStatic);
        }
        else if (background == 'BingRoadDynamic') {
          layer.setSource(component.backgrounds.Source_BingRoadDynamic);
        }
        else if (background == 'HERENormalDay') {
          layer.setSource(component.backgrounds.Source_HERENormalDay);
        }
        else if (background == 'HERENormalDayTransit') {
          layer.setSource(component.backgrounds.Source_HERENormalDayTransit);
        }
        else if (background == 'HERETerrainDay') {
          layer.setSource(component.backgrounds.Source_HERETerrainDay);
        }
        else if (background == 'HEREHybridDay') {
          layer.setSource(component.backgrounds.Source_HEREHybridDay);
        }
        else if (background == 'StamenWatercolor') {
          layer.setSource(component.backgrounds.Source_StamenWatercolor);
        }
        else if (background == 'StamenTerrain') {
          layer.setSource(component.backgrounds.Source_StamenTerrain);
        }
        else if (background == 'StamenToner') {
          layer.setSource(component.backgrounds.Source_StamenToner);
        }
        else if (background == 'ArcGIS') {
          layer.setSource(component.backgrounds.Source_ArcGIS);
        }
        else if (background == 'ThunderforestOpenCycleMap') {
          layer.setSource(component.backgrounds.Source_ThunderforestOpenCycleMap);
        }
        else if (background == 'ThunderforestTransport') {
          layer.setSource(component.backgrounds.Source_ThunderforestTransport);
        }
        else if (background == 'ThunderforestLandscape') {
          layer.setSource(component.backgrounds.Source_ThunderforestLandscape);
        }
        else if (background == 'ThunderforestOutdoors') {
          layer.setSource(component.backgrounds.Source_ThunderforestOutdoors);
        }
        else if (background == 'ThunderforestTransportDark') {
          layer.setSource(component.backgrounds.Source_ThunderforestTransportDark);
        }
        else if (background == 'ThunderforestSpinalMap') {
          layer.setSource(component.backgrounds.Source_ThunderforestSpinalMap);
        }
        else if (background == 'ThunderforestPioneer') {
          layer.setSource(component.backgrounds.Source_ThunderforestPioneer);
        }
        else if (background == 'ThunderforestMobileAtlas') {
          layer.setSource(component.backgrounds.Source_ThunderforestMobileAtlas);
        }
        else if (background == 'ThunderforestNeighbourhood') {
          layer.setSource(component.backgrounds.Source_ThunderforestNeighbourhood);
        }
      }
    })
  }

  private HEREcreateUrl(template, HERElayerDesc) {
    return template
      .replace('{base}', HERElayerDesc.base)
      .replace('{type}', HERElayerDesc.type)
      .replace('{scheme}', HERElayerDesc.scheme)
      .replace('{app_id}', HERElayerDesc.app_id)
      .replace('{app_code}', HERElayerDesc.app_code);
  };
}
