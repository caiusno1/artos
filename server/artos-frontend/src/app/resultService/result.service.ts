import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ExperimentalResult } from './ExperimentalResult';

@Injectable({
  providedIn: 'root'
})
export class ResultService {
  private concatedDomain: string = "localhost"
  constructor(private http: HttpClient) {
    if(!environment.devEnvironment){
      this.concatedDomain = `https://artosapi.${environment.domain}`
    }
    else {
      this.concatedDomain = `http://${environment.domain}`
    }
  }
  getResults(): Promise<ExperimentalResult[]>{
    return this.http.get<ExperimentalResult[]>(`${this.concatedDomain}/results`).toPromise().then((data) => {
      return data
    })
  }
  deleteResult(result:ExperimentalResult): Promise<ExperimentalResult[]>{
    return this.http.delete(`${this.concatedDomain}/results`,{body:result}).toPromise().then((data) => {
      return this.getResults()
    })
  }
  createJuypterNotebook(expermimentID: number, participantIDName: string, conditionIDNames: string[]){
    return this.http.put(`https://kai-biermeier.de/jupyter/`,
    {
      "expermimentID":`${expermimentID}`,
      "participantID":`${participantIDName}`,
      "conditionIDs": conditionIDNames
    }
    ).toPromise().then((data) => {return data})
  }
}
