import { Component } from '@angular/core';
import { UserService } from '../users/user.service';

@Component({
  templateUrl: 'reports.component.html'
})

export class ReportsComponent {
  constructor(public userService: UserService) { }
}
