import { AccountService } from './../_services/account.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model:any ={};

  constructor(private accountService:AccountService) { }

  ngOnInit():void {}

  register(){
    this.accountService.register(this.model).subscribe(user=>{
      console.log(user);
      this.cancel();      
    }, error => console.log(error))
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
