import { Component } from '@angular/core';
import { UserService } from '../users/user.service';

@Component({
  templateUrl: 'administration.component.html'
})

export class AdministrationComponent {
  constructor(public userService: UserService) { }
}
