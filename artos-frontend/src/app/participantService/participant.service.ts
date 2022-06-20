import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DB_Participant } from './participant';

@Injectable({
  providedIn: 'root'
})
export class ParticipantService {

  constructor(private http: HttpClient) { }
  getParticipantID(expermimentID: number){
    return this.http.get<DB_Participant>(`http://localhost:8000/participant/${expermimentID}`
    ).toPromise().then((data) => {return data.name})
  }
  updateParticipant(experimentID: number, currentParticipant: string){

    const participant: Partial<DB_Participant> = {experiment: {ID: experimentID}, name:currentParticipant}
    return this.http.post<void>(`http://localhost:8000/participant/${experimentID}`, participant).subscribe((res) =>{})
  }
}
