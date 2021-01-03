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
import { LoginComponent } from './users/login.component';
import { ChangePasswordComponent } from './users/changepassword.component';
import { UsersIndexComponent } from './users/index.component';
import { UsersListComponent } from './users/list.component';
import { UserDetailsComponent } from './users/details.component';
import { UserEditComponent } from './users/edit.component';

import { AdministrationComponent } from './administration/administration.component';

import { PollutionEnvironmentService } from './pollutionenvironments/pollutionenvironment.service';
import { PollutionEnvironmentsIndexComponent } from './pollutionenvironments/index.component';
import { PollutionEnvironmentsListComponent } from './pollutionenvironments/list.component';
import { PollutionEnvironmentCreateComponent } from './pollutionenvironments/create.component';
import { PollutionEnvironmentEditComponent } from './pollutionenvironments/edit.component';
import { PollutionEnvironmentDetailsComponent } from './pollutionenvironments/details.component';

import { MeasuredParameterService } from './measuredparameters/measuredparameter.service';
import { MeasuredParametersIndexComponent } from './measuredparameters/index.component';
import { MeasuredParametersListComponent } from './measuredparameters/list.component';
import { MeasuredParameterCreateComponent } from './measuredparameters/create.component';
import { MeasuredParameterEditComponent } from './measuredparameters/edit.component';
import { MeasuredParameterDetailsComponent } from './measuredparameters/details.component';

import { DataProviderService } from './dataproviders/dataprovider.service';
import { DataProvidersIndexComponent } from './dataproviders/index.component';
import { DataProvidersListComponent } from './dataproviders/list.component';
import { DataProviderCreateComponent } from './dataproviders/create.component';
import { DataProviderEditComponent } from './dataproviders/edit.component';
import { DataProviderDetailsComponent } from './dataproviders/details.component';

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
    LoginComponent,
    ChangePasswordComponent,
    UsersIndexComponent,
    UsersListComponent,
    UserDetailsComponent,
    UserEditComponent,
    AdministrationComponent,
    DashboardComponent,
    PollutionEnvironmentsIndexComponent,
    PollutionEnvironmentsListComponent,
    PollutionEnvironmentCreateComponent,
    PollutionEnvironmentEditComponent,
    PollutionEnvironmentDetailsComponent,
    MeasuredParametersIndexComponent,
    MeasuredParametersListComponent,
    MeasuredParameterCreateComponent,
    MeasuredParameterEditComponent,
    MeasuredParameterDetailsComponent,
    DataProvidersIndexComponent,
    DataProvidersListComponent,
    DataProviderCreateComponent,
    DataProviderEditComponent,
    DataProviderDetailsComponent
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
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (createTranslateLoader),
        deps: [HttpClient]
      }
    })
  ],
  providers: [
    UserService,
    PollutionEnvironmentService,
    MeasuredParameterService,
    DataProviderService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthorizeInterceptor,
      multi: true
    },
    {
      provide: MatPaginatorIntl,
      useClass: Paginator
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
