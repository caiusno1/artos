import { AuthService } from './authentificationService/auth.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'artos-frontend';
  isLoggedIn = false
  constructor(public auth:AuthService){
  }
  public logout(){
    console.log("test")
    this.auth.logout()
  }
}
