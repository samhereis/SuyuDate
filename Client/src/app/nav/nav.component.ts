import { Component, OnInit } from '@angular/core';
import { User } from '../models/user';
import { AccountService } from '../services/account.service';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {

  model: any = {};
  currentUser$: Observable<User | null> = of(null);

  constructor(public accountService: AccountService) {

  }

  ngOnInit(): void {
    
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: responce => {
        console.log(responce)
      },
      error: error => console.log(error)
    })
  }

  logout() {
    this.accountService.logout();
  }
}
