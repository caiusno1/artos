import { environment } from './../../environments/environment';
/**
 * This service manage all authentication related stuff such as asking the server for an JWT Token, providing the stored username ...
 */
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private concatedDomain: string = "localhost"
  // tslint:disable-next-line:variable-name
  private _username: string | undefined;
  public createUser(email: string, name: string, password: string, age: number, job: string,hobbies: string, aboutMe: string) {
    return this.http.post(`${this.concatedDomain}/register`,
    {email, name, password})
    .toPromise()
    .then((res) => {
      if ((res as any).status === 0){
        this.router.navigate(['/login']);
      }
    })
    .catch((err) => Promise.reject());
  }

  constructor(private http: HttpClient, private router: Router) {
    if(!environment.devEnvironment){
      this.concatedDomain = `https://artosapi.${environment.domain}`
    }
    else {
      this.concatedDomain = `http://${environment.domain}`
    }
  }

  public isAuthenticated(): boolean {
    const userData = localStorage.getItem('id_token');
    if (userData ){
      return true;
    }
    return false;
  }
  public authStatusObservable: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.isAuthenticated())

  public setUserInfo(token:string){
    localStorage.setItem('id_token', token);
  }

  public logout(){
    localStorage.clear();
    this.router.navigate(['/']);
    this.authStatusObservable.next(false)
  }

  public validate(username:string, password:string) {
    return this.http.post(`${this.concatedDomain}/authenticate`, {username, password })
    .toPromise()
    .then((valid) => {

      if (valid){
        this._username = username;
        localStorage.setItem('username', username);
        this.authStatusObservable.next(true)
      }
      return valid;
    }).catch(
    (err) => Promise.reject());
  }

  public getMyUsername(): string {
    if (this._username){
      return this._username;
    } else {
      return localStorage.getItem('username') as string;
    }

  }
}
