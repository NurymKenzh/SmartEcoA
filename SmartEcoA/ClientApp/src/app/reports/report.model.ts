import { User } from "../users/user.model";

export class Report {
  Id: number;
  ApplicationUser: User;
  Name: string;
  NameEN: string;
  NameRU: string;
  NameKK: string;
  InputParameters: string;
  InputParametersEN: string;
  InputParametersRU: string;
  InputParametersKK: string;
  Inputs: string;
  DateTime?: Date;
  CarPostStartDate?: Date;
  CarPostEndDate?: Date;
  FileName: string;
  PDF: boolean;
}
