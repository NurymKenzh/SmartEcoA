import { Project } from '../projects/project.model';
import { PollutionEnvironment } from '../pollutionenvironments/pollutionenvironment.model';
import { DataProvider } from '../dataproviders/dataprovider.model';

export class Post {
  Id: number;
  Name: string;
  MN: string;
  Latitude: number;
  Longitude: number;
  Information: string;
  PhoneNumber: string;
  ProjectId: bigint;
  Project: Project;
  PollutionEnvironmentId: bigint;
  PollutionEnvironment: PollutionEnvironment;
  DataProviderId: bigint;
  DataProvider: DataProvider;
  KazhydrometID: bigint;
  Automatic: boolean;
}
