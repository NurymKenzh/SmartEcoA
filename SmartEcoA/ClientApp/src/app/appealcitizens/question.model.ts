import { User } from "../users/user.model";

export class Question {
  Id: number;
  Name: string;
  Text: string;
  DateTime: Date;
  ApplicationUser: User;
}
