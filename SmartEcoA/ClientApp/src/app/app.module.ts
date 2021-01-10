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
import 'hammerjs';

import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { DashboardComponent } from './dashboard/dashboard.component';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
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

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/locale/', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
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
    DashboardComponent,
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
    PostDataDetailsComponent
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
      { path: 'dashboard', component: DashboardComponent, canActivate: [AuthorizeGuard], data: { allowedRoles: ['Administrator', 'Moderator', 'Customer'] } },
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
    PollutionEnvironmentService,
    MeasuredParameterService,
    DataProviderService,
    ProjectService,
    PostService,
    PostDataService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthorizeInterceptor,
      multi: true
    },
    {
      provide: MatPaginatorIntl,
      useClass: Paginator
    }],
  entryComponents: [
    UserDeleteComponent,
    RegisterInfoComponent,
    ChangePasswordInfoComponent,
    PollutionEnvironmentDeleteComponent,
    MeasuredParameterDeleteComponent,
    DataProviderDeleteComponent,
    ProjectDeleteComponent,
    PostDeleteComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
