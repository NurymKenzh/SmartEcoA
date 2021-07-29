import { Component } from '@angular/core';
import { UserService } from '../users/user.service';

@Component({
  templateUrl: 'create.component.html'
})

export class ReportCreateComponent {
  constructor(public userService: UserService) { }
}
