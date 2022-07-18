import { Experiment } from './Experiment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ExperimentService {
  private concatedDomain: string = "localhost"
  constructor(private http:HttpClient) {
    if(!environment.devEnvironment){
      this.concatedDomain = `https://artosapi.${environment.domain}`
    }
    else {
      this.concatedDomain = `http://${environment.domain}`
    }
  }
  public getExperiments(){
    return this.http.get<Experiment[]>(`${this.concatedDomain}/experiments`).toPromise().then((data) => {
      return data
    })
  }
  public createNewExperiment(){
    return this.http.put<Experiment[]>(`${this.concatedDomain}/experiments`,{}).toPromise().then(() => {})
  }
  deleteExperiment(experimentID: number) {
    return this.http.delete<Experiment[]>(`${this.concatedDomain}/experiments/${experimentID}`,{}).toPromise().then(() => {})
  }
}
