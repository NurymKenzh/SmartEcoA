import { CarPost } from '../carposts/carpost.model';

export class CarModelSmokeMeter {
  Id: number;
  Name: string;
  Boost: boolean;
  DFreeMark: number;
  DMaxMark: number;
  CarPostId: bigint;
  CarPost: CarPost;
}
