import { environment } from 'src/environments/environment';
import { AuthService } from './authentificationService/auth.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'artos-frontend';
  version = "undefined";
  isLoggedIn = false
  constructor(public auth:AuthService){
    this.version = environment.version
  }
  public logout(){
    console.log("test")
    this.auth.logout()
  }
}
