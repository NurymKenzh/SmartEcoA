import { Component } from '@angular/core';
import { UserService } from '../users/user.service';

@Component({
  templateUrl: 'createcarposts.component.html'
})

export class ReportCreateCarPostsComponent {
  constructor(public userService: UserService) { }
}
