import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ExperimentalResult } from './ExperimentalResult';

@Injectable({
  providedIn: 'root'
})
export class ResultService {

  constructor(private http: HttpClient) { }
  getResults(): Promise<ExperimentalResult[]>{
    return this.http.get<ExperimentalResult[]>(`https://artosapi.${environment.domain}/results`).toPromise().then((data) => {
      return data
    })
  }
  deleteResult(result:ExperimentalResult): Promise<ExperimentalResult[]>{
    return this.http.delete(`https://artosapi.${environment.domain}/results`,{body:result}).toPromise().then((data) => {
      return this.getResults()
    })
  }
}
