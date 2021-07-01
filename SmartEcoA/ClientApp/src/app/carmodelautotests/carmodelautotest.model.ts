import { CarPost } from '../carposts/carpost.model';

export class CarModelAutoTest {
  Id: number;
  Name: string;
  EngineType: number;
  MIN_TAH: number;
  DEL_MIN: number;
  MAX_TAH: number;
  DEL_MAX: number;
  MIN_CO: number;
  MAX_CO: number;
  MIN_CH: number;
  MAX_CH: number;
  L_MIN: number;
  L_MAX: number;
  K_SVOB: number;
  K_MAX: number;
  MIN_CO2: number;
  MIN_O2: number;
  MIN_NOx: number;
  MAX_CO2: number;
  MAX_O2: number;
  MAX_NOx: number;
  Version: number;
  CarPostId: bigint;
  CarPost: CarPost;
}
