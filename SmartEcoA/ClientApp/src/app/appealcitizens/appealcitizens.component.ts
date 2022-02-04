import { Component } from '@angular/core';
import { UserService } from '../users/user.service';

@Component({
  templateUrl: 'appealcitizens.component.html'
})

export class AppealCitizensComponent {
  constructor(public userService: UserService) { }
}
