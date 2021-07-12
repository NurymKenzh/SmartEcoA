import { Component } from '@angular/core';
import { UserService } from '../users/user.service';

@Component({
  templateUrl: './dashboard.component.html'
})

export class DashboardComponent {
  constructor(public userService: UserService) { }
}
