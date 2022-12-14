import { User } from './../_models/user';
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  model:any ={}

  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
  }

  login(){
    this.accountService.login(this.model).subscribe( response=> { 
      console.log(response);
      },
      error => {
        console.error(error);
      }
    )  
  }

  logout(){
    this.accountService.logout();
  }

}
