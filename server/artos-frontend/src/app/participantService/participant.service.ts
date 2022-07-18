import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { DB_Participant } from './participant';

@Injectable({
  providedIn: 'root'
})
export class ParticipantService {
  private concatedDomain!: string;

  constructor(private http: HttpClient) {
    if(!environment.devEnvironment){
      this.concatedDomain = `https://artosapi.${environment.domain}`
    }
    else {
      this.concatedDomain = `http://${environment.domain}`
    }
  }
  getParticipantID(expermimentID: number){

    return this.http.get<DB_Participant>(`${this.concatedDomain}/participant/${expermimentID}`
    ).toPromise().then((data) => {return data.name})
  }
  updateParticipant(experimentID: number, currentParticipant: string){

    const participant: Partial<DB_Participant> = {experiment: {ID: experimentID}, name:currentParticipant}
    return this.http.post<void>(`${this.concatedDomain}/participant/${experimentID}`, participant).subscribe((res) =>{})
  }
}
