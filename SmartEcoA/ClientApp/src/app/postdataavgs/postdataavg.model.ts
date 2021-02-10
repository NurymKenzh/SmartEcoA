import { MeasuredParameter } from "../measuredparameters/measuredparameter.model";
import { Post } from "../posts/post.model";

export class PostDataAvg {
  Id: number;
  DateTime: Date;
  Value: number;
  MeasuredParameterId: bigint;
  MeasuredParameter: MeasuredParameter;
  PostId: bigint;
  Post: Post;
}
