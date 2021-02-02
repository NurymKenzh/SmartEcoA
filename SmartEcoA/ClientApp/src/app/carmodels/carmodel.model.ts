import { CarPost } from '../carposts/carpost.model';

export class CarModel {
  Id: number;
  Name: string;
  Boost: boolean;
  DFreeMark: number;
  DMaxMark: number;
  CarPostId: bigint;
  CarPost: CarPost;
}
