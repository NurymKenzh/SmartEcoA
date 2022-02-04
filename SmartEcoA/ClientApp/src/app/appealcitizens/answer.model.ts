import { User } from "../users/user.model";
import { Question } from "../appealcitizens/question.model";

export class Answer {
  Id: number;
  Text: string;
  DateTime: Date;
  ApplicationUser: User;
  Question: Question;
}
