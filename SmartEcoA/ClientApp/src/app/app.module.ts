import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, MatPaginatorIntl } from '@angular/material/paginator';
import { Paginator } from './paginator/paginator.component';
import { MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatMenuModule } from '@angular/material/menu';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDatetimepickerModule, MatNativeDatetimeModule } from "@mat-datetimepicker/core";
//import { MatMomentDatetimeModule } from "@mat-datetimepicker/moment";
//import { MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import 'hammerjs';

import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';

import { LocaleIndexComponent } from './locale/index.component';

import { AuthorizeInterceptor } from './authorize/authorize.interceptor';
import { AuthorizeGuard } from './authorize/authorize.guard';

import { UserService } from './users/user.service';
import { RegisterComponent } from './users/register.component';
import { RegisterInfoComponent } from './users/registerinfo.component';
import { LoginComponent } from './users/login.component';
import { ChangePasswordComponent } from './users/changepassword.component';
import { ChangePasswordInfoComponent } from './users/changepasswordinfo.component';
import { UsersIndexComponent } from './users/index.component';
import { UsersListComponent } from './users/list.component';
import { UserDeleteComponent } from './users/delete.component';
import { UserDetailsComponent } from './users/details.component';
import { UserEditComponent } from './users/edit.component';

import { OLService } from './ol/ol.service';

import { AdministrationComponent } from './administration/administration.component';
import { DirectoriesComponent } from './administration/directories.component';
import { PostsComponent } from './administration/posts.component';
import { CarPostsComponent } from './administration/carposts.component';

import { DashboardComponent } from './dashboard/dashboard.component';
import { DashboardPostsComponent } from './dashboard/dashboardposts.component';
import { DashboardCarPostsComponent } from './dashboard/dashboardcarposts.component';

import { ReportsComponent } from './reports/reports.component';

import { ReportService } from './reports/report.service';
import { ReportsListComponent } from './reports/list.component';
import { ReportDeleteComponent } from './reports/delete.component';
import { ReportCreateComponent } from './reports/create.component';
import { ReportCreateCarPostsComponent } from './reports/createcarposts.component';
import { ReportCreateCarPostDataAutoTestProtocolComponent } from './reports/createcarpostdataautotestprotocol.component';
import { ReportCreateCarPostDataSmokeMeterProtocolComponent } from './reports/createcarpostdatasmokemeterprotocol.component';
import { ReportCreateCarPostDataSmokeMeterLogComponent } from './reports/createcarpostdatasmokemeterlog.component';
import { ReportCreateCarPostDataAutoTestLogComponent } from './reports/createcarpostdataautotestlog.component';
import { ReportCreateCarPostsProtocolComponent } from './reports/createcarpostsprotocol.component';
import { ReportCreateCarsExcessProtocolComponent } from './reports/createcarsexcessprotocol.component';
import { ReportDetailsComponent } from './reports/details.component';

import { PollutionEnvironmentService } from './pollutionenvironments/pollutionenvironment.service';
import { PollutionEnvironmentsIndexComponent } from './pollutionenvironments/index.component';
import { PollutionEnvironmentsListComponent } from './pollutionenvironments/list.component';
import { PollutionEnvironmentDeleteComponent } from './pollutionenvironments/delete.component';
import { PollutionEnvironmentCreateComponent } from './pollutionenvironments/create.component';
import { PollutionEnvironmentEditComponent } from './pollutionenvironments/edit.component';
import { PollutionEnvironmentDetailsComponent } from './pollutionenvironments/details.component';

import { MeasuredParameterService } from './measuredparameters/measuredparameter.service';
import { MeasuredParametersIndexComponent } from './measuredparameters/index.component';
import { MeasuredParametersListComponent } from './measuredparameters/list.component';
import { MeasuredParameterDeleteComponent } from './measuredparameters/delete.component';
import { MeasuredParameterCreateComponent } from './measuredparameters/create.component';
import { MeasuredParameterEditComponent } from './measuredparameters/edit.component';
import { MeasuredParameterDetailsComponent } from './measuredparameters/details.component';

import { DataProviderService } from './dataproviders/dataprovider.service';
import { DataProvidersIndexComponent } from './dataproviders/index.component';
import { DataProvidersListComponent } from './dataproviders/list.component';
import { DataProviderDeleteComponent } from './dataproviders/delete.component';
import { DataProviderCreateComponent } from './dataproviders/create.component';
import { DataProviderEditComponent } from './dataproviders/edit.component';
import { DataProviderDetailsComponent } from './dataproviders/details.component';

import { ProjectService } from './projects/project.service';
import { ProjectsIndexComponent } from './projects/index.component';
import { ProjectsListComponent } from './projects/list.component';
import { ProjectDeleteComponent } from './projects/delete.component';
import { ProjectCreateComponent } from './projects/create.component';
import { ProjectEditComponent } from './projects/edit.component';
import { ProjectDetailsComponent } from './projects/details.component';

import { PostService } from './posts/post.service';
import { PostsIndexComponent } from './posts/index.component';
import { PostsListComponent } from './posts/list.component';
import { PostDeleteComponent } from './posts/delete.component';
import { PostCreateComponent } from './posts/create.component';
import { PostEditComponent } from './posts/edit.component';
import { PostDetailsComponent } from './posts/details.component';

import { PostDataService } from './postdatas/postdata.service';
import { PostDatasIndexComponent } from './postdatas/index.component';
import { PostDatasListComponent } from './postdatas/list.component';
import { PostDataDetailsComponent } from './postdatas/details.component';

import { PostDataDividedService } from './postdatadivideds/postdatadivided.service';
import { PostDataDividedsIndexComponent } from './postdatadivideds/index.component';
import { PostDataDividedsListComponent } from './postdatadivideds/list.component';
import { PostDataDividedDetailsComponent } from './postdatadivideds/details.component';

import { PostDataAvgService } from './postdataavgs/postdataavg.service';
import { PostDataAvgsIndexComponent } from './postdataavgs/index.component';
import { PostDataAvgsListComponent } from './postdataavgs/list.component';
import { PostDataAvgDetailsComponent } from './postdataavgs/details.component';

import { CarPostService } from './carposts/carpost.service';
import { CarPostsIndexComponent } from './carposts/index.component';
import { CarPostsListComponent } from './carposts/list.component';
import { CarPostDeleteComponent } from './carposts/delete.component';
import { CarPostCreateComponent } from './carposts/create.component';
import { CarPostEditComponent } from './carposts/edit.component';
import { CarPostDetailsComponent } from './carposts/details.component';

import { CarModelSmokeMeterService } from './carmodelsmokemeters/carmodelsmokemeter.service';
import { CarModelSmokeMetersIndexComponent } from './carmodelsmokemeters/index.component';
import { CarModelSmokeMetersListComponent } from './carmodelsmokemeters/list.component';
import { CarModelSmokeMeterDeleteComponent } from './carmodelsmokemeters/delete.component';
import { CarModelSmokeMeterCreateComponent } from './carmodelsmokemeters/create.component';
import { CarModelSmokeMeterEditComponent } from './carmodelsmokemeters/edit.component';
import { CarModelSmokeMeterDetailsComponent } from './carmodelsmokemeters/details.component';

import { CarPostDataSmokeMeterService } from './carpostdatasmokemeters/carpostdatasmokemeter.service';
import { CarPostDataSmokeMetersIndexComponent } from './carpostdatasmokemeters/index.component';
import { CarPostDataSmokeMetersListComponent } from './carpostdatasmokemeters/list.component';
import { CarPostDataSmokeMeterDeleteComponent } from './carpostdatasmokemeters/delete.component';
import { CarPostDataSmokeMeterCreateComponent } from './carpostdatasmokemeters/create.component';
import { CarPostDataSmokeMeterEditComponent } from './carpostdatasmokemeters/edit.component';
import { CarPostDataSmokeMeterDetailsComponent } from './carpostdatasmokemeters/details.component';

import { CarModelAutoTestService } from './carmodelautotests/carmodelautotest.service';
import { CarModelAutoTestsIndexComponent } from './carmodelautotests/index.component';
import { CarModelAutoTestsListComponent } from './carmodelautotests/list.component';
import { CarModelAutoTestDeleteComponent } from './carmodelautotests/delete.component';
import { CarModelAutoTestCreateComponent } from './carmodelautotests/create.component';
import { CarModelAutoTestEditComponent } from './carmodelautotests/edit.component';
import { CarModelAutoTestDetailsComponent } from './carmodelautotests/details.component';

import { CarPostDataAutoTestService } from './carpostdataautotests/carpostdataautotest.service';
import { CarPostDataAutoTestsIndexComponent } from './carpostdataautotests/index.component';
import { CarPostDataAutoTestsListComponent } from './carpostdataautotests/list.component';
import { CarPostDataAutoTestDeleteComponent } from './carpostdataautotests/delete.component';
import { CarPostDataAutoTestCreateComponent } from './carpostdataautotests/create.component';
import { CarPostDataAutoTestEditComponent } from './carpostdataautotests/edit.component';
import { CarPostDataAutoTestDetailsComponent } from './carpostdataautotests/details.component';

import { TypeEcoClassService } from './typeecoclasses/typeecoclass.service';
import { TypeEcoClassesIndexComponent } from './typeecoclasses/index.component';
import { TypeEcoClassesListComponent } from './typeecoclasses/list.component';
import { TypeEcoClassDeleteComponent } from './typeecoclasses/delete.component';
import { TypeEcoClassCreateComponent } from './typeecoclasses/create.component';
import { TypeEcoClassEditComponent } from './typeecoclasses/edit.component';
import { TypeEcoClassDetailsComponent } from './typeecoclasses/details.component';

import { TesterService } from './testers/tester.service';
import { TestersIndexComponent } from './testers/index.component';
import { TestersListComponent } from './testers/list.component';
import { TesterDeleteComponent } from './testers/delete.component';
import { TesterCreateComponent } from './testers/create.component';
import { TesterEditComponent } from './testers/edit.component';
import { TesterDetailsComponent } from './testers/details.component';

import { CarPostAnalyticService } from './carpostanalytics/carpostanalytic.service';
import { CarPostAnalyticsIndexComponent } from './carpostanalytics/index.component';
import { CarPostAnalyticsListComponent } from './carpostanalytics/list.component';
import { CarPostAnalyticDeleteComponent } from './carpostanalytics/delete.component';
import { CarPostAnalyticCreateComponent } from './carpostanalytics/create.component';
import { CarPostAnalyticEditComponent } from './carpostanalytics/edit.component';
import { CarPostAnalyticDetailsComponent } from './carpostanalytics/details.component';

import { MatTableExporterModule } from 'mat-table-exporter';
import { NgxPrintModule } from 'ngx-print';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/locale/', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LocaleIndexComponent,
    RegisterComponent,
    RegisterInfoComponent,
    LoginComponent,
    ChangePasswordComponent,
    UsersIndexComponent,
    UsersListComponent,
    UserDeleteComponent,
    ChangePasswordInfoComponent,
    UserDetailsComponent,
    UserEditComponent,
    AdministrationComponent,
    DirectoriesComponent,
    PostsComponent,
    CarPostsComponent,
    DashboardComponent,
    DashboardPostsComponent,
    DashboardCarPostsComponent,
    ReportsComponent,
    ReportsListComponent,
    ReportCreateComponent,
    ReportCreateCarPostsComponent,
    ReportCreateCarPostDataAutoTestProtocolComponent,
    ReportCreateCarPostDataSmokeMeterProtocolComponent,
    ReportCreateCarPostDataSmokeMeterLogComponent,
    ReportCreateCarPostDataAutoTestLogComponent,
    ReportCreateCarPostsProtocolComponent,
    ReportCreateCarsExcessProtocolComponent,
    ReportDeleteComponent,
    ReportDetailsComponent,
    PollutionEnvironmentsIndexComponent,
    PollutionEnvironmentsListComponent,
    PollutionEnvironmentDeleteComponent,
    PollutionEnvironmentCreateComponent,
    PollutionEnvironmentEditComponent,
    PollutionEnvironmentDetailsComponent,
    MeasuredParametersIndexComponent,
    MeasuredParametersListComponent,
    MeasuredParameterDeleteComponent,
    MeasuredParameterCreateComponent,
    MeasuredParameterEditComponent,
    MeasuredParameterDetailsComponent,
    DataProvidersIndexComponent,
    DataProvidersListComponent,
    DataProviderDeleteComponent,
    DataProviderCreateComponent,
    DataProviderEditComponent,
    DataProviderDetailsComponent,
    ProjectsIndexComponent,
    ProjectsListComponent,
    ProjectDeleteComponent,
    ProjectCreateComponent,
    ProjectEditComponent,
    ProjectDetailsComponent,
    PostsIndexComponent,
    PostsListComponent,
    PostDeleteComponent,
    PostCreateComponent,
    PostEditComponent,
    PostDetailsComponent,
    PostDatasIndexComponent,
    PostDatasListComponent,
    PostDataDetailsComponent,
    PostDataDividedsIndexComponent,
    PostDataDividedsListComponent,
    PostDataDividedDetailsComponent,
    PostDataAvgsIndexComponent,
    PostDataAvgsListComponent,
    PostDataAvgDetailsComponent,
    CarPostsIndexComponent,
    CarPostsListComponent,
    CarPostDeleteComponent,
    CarPostCreateComponent,
    CarPostEditComponent,
    CarPostDetailsComponent,
    CarModelSmokeMetersIndexComponent,
    CarModelSmokeMetersListComponent,
    CarModelSmokeMeterDeleteComponent,
    CarModelSmokeMeterCreateComponent,
    CarModelSmokeMeterEditComponent,
    CarModelSmokeMeterDetailsComponent,
    CarPostDataSmokeMetersIndexComponent,
    CarPostDataSmokeMetersListComponent,
    CarPostDataSmokeMeterDeleteComponent,
    CarPostDataSmokeMeterCreateComponent,
    CarPostDataSmokeMeterEditComponent,
    CarPostDataSmokeMeterDetailsComponent,
    CarModelAutoTestsIndexComponent,
    CarModelAutoTestsListComponent,
    CarModelAutoTestDeleteComponent,
    CarModelAutoTestCreateComponent,
    CarModelAutoTestEditComponent,
    CarModelAutoTestDetailsComponent,
    CarPostDataAutoTestsIndexComponent,
    CarPostDataAutoTestsListComponent,
    CarPostDataAutoTestDeleteComponent,
    CarPostDataAutoTestCreateComponent,
    CarPostDataAutoTestEditComponent,
    CarPostDataAutoTestDetailsComponent,
    TypeEcoClassesIndexComponent,
    TypeEcoClassesListComponent,
    TypeEcoClassDeleteComponent,
    TypeEcoClassCreateComponent,
    TypeEcoClassEditComponent,
    TypeEcoClassDetailsComponent,
    TestersIndexComponent,
    TestersListComponent,
    TesterDeleteComponent,
    TesterCreateComponent,
    TesterEditComponent,
    TesterDetailsComponent,
    CarPostAnalyticsIndexComponent,
    CarPostAnalyticsListComponent,
    CarPostAnalyticDeleteComponent,
    CarPostAnalyticCreateComponent,
    CarPostAnalyticEditComponent,
    CarPostAnalyticDetailsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'locale', component: LocaleIndexComponent },
      { path: 'users/register', component: RegisterComponent },
      { path: 'users/login', component: LoginComponent },
      { path: 'users/changepassword', component: ChangePasswordComponent },
      { path: 'users', component: UsersIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator'] } },
      { path: 'users/:id', component: UserDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator'] } },
      { path: 'users/edit/:id', component: UserEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator'] } },
      { path: 'administration', component: AdministrationComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'administration/directories', component: DirectoriesComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'administration/posts', component: PostsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'administration/carposts', component: CarPostsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'dashboard', component: DashboardComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'dashboard/dashboardposts', component: DashboardPostsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'dashboard/dashboardcarposts', component: DashboardCarPostsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports', component: ReportsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports/create', component: ReportCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports/create/carposts', component: ReportCreateCarPostsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports/create/carposts/createcarpostdataautotestprotocol', component: ReportCreateCarPostDataAutoTestProtocolComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports/create/carposts/createcarpostdatasmokemeterprotocol', component: ReportCreateCarPostDataSmokeMeterProtocolComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports/create/carposts/createcarpostdatasmokemeterlog', component: ReportCreateCarPostDataSmokeMeterLogComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports/create/carposts/createcarpostdataautotestlog', component: ReportCreateCarPostDataAutoTestLogComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports/create/carposts/createcarpostsprotocol', component: ReportCreateCarPostsProtocolComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports/create/carposts/createcarsexcessprotocol', component: ReportCreateCarsExcessProtocolComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
      { path: 'reports/:id', component: ReportDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'pollutionenvironments', component: PollutionEnvironmentsIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'pollutionenvironments/create', component: PollutionEnvironmentCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'pollutionenvironments/edit/:id', component: PollutionEnvironmentEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'pollutionenvironments/:id', component: PollutionEnvironmentDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'measuredparameters', component: MeasuredParametersIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'measuredparameters/create', component: MeasuredParameterCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'measuredparameters/edit/:id', component: MeasuredParameterEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'measuredparameters/:id', component: MeasuredParameterDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'dataproviders', component: DataProvidersIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'dataproviders/create', component: DataProviderCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'dataproviders/edit/:id', component: DataProviderEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'dataproviders/:id', component: DataProviderDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'projects', component: ProjectsIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'projects/create', component: ProjectCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'projects/edit/:id', component: ProjectEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'projects/:id', component: ProjectDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'posts', component: PostsIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'posts/create', component: PostCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'posts/edit/:id', component: PostEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'posts/:id', component: PostDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'postdatas', component: PostDatasIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'postdatas/:id', component: PostDataDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'postdatadivideds', component: PostDataDividedsIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'postdatadivideds/:id', component: PostDataDividedDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'postdataavgs', component: PostDataAvgsIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'postdataavgs/:id', component: PostDataAvgDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carposts', component: CarPostsIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carposts/create', component: CarPostCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carposts/edit/:id', component: CarPostEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carposts/:id', component: CarPostDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carmodelsmokemeters', component: CarModelSmokeMetersIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carmodelsmokemeters/create', component: CarModelSmokeMeterCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carmodelsmokemeters/edit/:id', component: CarModelSmokeMeterEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carmodelsmokemeters/:id', component: CarModelSmokeMeterDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostdatasmokemeters', component: CarPostDataSmokeMetersIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostdatasmokemeters/create', component: CarPostDataSmokeMeterCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostdatasmokemeters/edit/:id', component: CarPostDataSmokeMeterEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostdatasmokemeters/:id', component: CarPostDataSmokeMeterDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carmodelautotests', component: CarModelAutoTestsIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carmodelautotests/create', component: CarModelAutoTestCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carmodelautotests/edit/:id', component: CarModelAutoTestEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carmodelautotests/:id', component: CarModelAutoTestDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostdataautotests', component: CarPostDataAutoTestsIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostdataautotests/create', component: CarPostDataAutoTestCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostdataautotests/edit/:id', component: CarPostDataAutoTestEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostdataautotests/:id', component: CarPostDataAutoTestDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'typeecoclasses', component: TypeEcoClassesIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'typeecoclasses/create', component: TypeEcoClassCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'typeecoclasses/edit/:id', component: TypeEcoClassEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'typeecoclasses/:id', component: TypeEcoClassDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'testers', component: TestersIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'testers/create', component: TesterCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'testers/edit/:id', component: TesterEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'testers/:id', component: TesterDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostanalytics', component: CarPostAnalyticsIndexComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostanalytics/create', component: CarPostAnalyticCreateComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostanalytics/edit/:id', component: CarPostAnalyticEditComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
      { path: 'carpostanalytics/:id', component: CarPostAnalyticDetailsComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator'] } },
    ]),
    BrowserAnimationsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatTooltipModule,
    MatCardModule,
    MatButtonModule,
    MatDividerModule,
    MatListModule,
    MatCheckboxModule,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatGridListModule,
    MatMenuModule,
    MatExpansionModule,
    MatTabsModule,
    MatDialogModule,
    MatSnackBarModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatDatetimepickerModule,
    MatNativeDatetimeModule,
    MatTableExporterModule,
    NgxPrintModule,
    //MatMomentDatetimeModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (createTranslateLoader),
        deps: [HttpClient]
      }
    })
  ],
  providers: [
    MatNativeDateModule,
    OLService,
    UserService,
    ReportService,
    PollutionEnvironmentService,
    MeasuredParameterService,
    DataProviderService,
    ProjectService,
    PostService,
    PostDataService,
    PostDataDividedService,
    PostDataAvgService,
    CarPostService,
    CarModelSmokeMeterService,
    CarPostDataSmokeMeterService,
    CarModelAutoTestService,
    CarPostDataAutoTestService,
    TypeEcoClassService,
    TesterService,
    CarPostAnalyticService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthorizeInterceptor,
      multi: true
    },
    {
      provide: MatPaginatorIntl,
      useClass: Paginator
    },
    //{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } }
  ],
  entryComponents: [
    UserDeleteComponent,
    RegisterInfoComponent,
    ChangePasswordInfoComponent,
    ReportDeleteComponent,
    PollutionEnvironmentDeleteComponent,
    MeasuredParameterDeleteComponent,
    DataProviderDeleteComponent,
    ProjectDeleteComponent,
    PostDeleteComponent,
    CarPostDeleteComponent,
    CarModelSmokeMeterDeleteComponent,
    CarPostDataSmokeMeterDeleteComponent,
    CarModelAutoTestDeleteComponent,
    CarPostDataAutoTestDeleteComponent,
    TypeEcoClassDeleteComponent,
    TesterDeleteComponent,
    CarPostAnalyticDeleteComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
