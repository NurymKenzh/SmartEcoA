import { CarModelSmokeMeter } from '../carmodelsmokemeters/carmodelsmokemeter.model';

export class CarPostDataSmokeMeter {
  Id: number;
  DateTime: Date;
  Number: string;
  RunIn: boolean;
  DFree: number;
  DMax: number;
  NDFree: number;
  NDMax: number;
  CarModelSmokeMeterId: bigint;
  CarModelSmokeMeter: CarModelSmokeMeter;
}
