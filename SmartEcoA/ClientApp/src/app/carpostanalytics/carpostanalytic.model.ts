import { CarPost } from '../carposts/carpost.model';

export class CarPostAnalytic {
  Id: number;
  Date: Date;
  Measurement: bigint;
  Exceeding: bigint;
  CarPostId: bigint;
  CarPost: CarPost;
}
