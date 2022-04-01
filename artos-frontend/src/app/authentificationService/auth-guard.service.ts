/**
 * A simple angular auth guard that prevents users from accessing pages they should only view if they are loged in.
 * No worry if they managed to bypass this authentification they will still have no valid JWT. Therefore they will get
 * no (private) data from the api server :).
 */
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(private authService: AuthService, private route: Router) { }

  canActivate(){
    if (this.authService.isAuthenticated()){
      return true;
    }
    this.route.navigate(['']);
    return false;
  }
}
