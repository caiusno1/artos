import { ParticipantService } from './../participantService/participant.service';
import { environment } from 'src/environments/environment';
import { ExperimentalResult } from './../resultService/ExperimentalResult';
import { BehaviorSubject } from 'rxjs';
import { ResultService } from './../resultService/result.service';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-experiment-view',
  templateUrl: './experiment-view.component.html',
  styleUrls: ['./experiment-view.component.scss']
})
export class ExperimentViewComponent implements OnInit {
  @Input() experimentID!: number;
  public currentParticipant: string = "VP00";
  public participant_id:string = "";
  public condition_id:string = "";
  displayedColumns: string[] = ['participant_id','timestamp','result','delete'];
  dataSource = new BehaviorSubject<ExperimentalResult[]>([{participant_id:0,result:"a",timestamp:"b"}])

  constructor(private resServ: ResultService, private participantServ: ParticipantService) {
    resServ.getResults().then((data) => {
      this.dataSource.next(data)
    })
  }
  public updateParticipant(curParticipant: string){
    this.currentParticipant = curParticipant
    console.log(this.currentParticipant)
    this.participantServ.updateParticipant(this.experimentID, this.currentParticipant)

  }

  public deleteElement(e:ExperimentalResult){
    this.resServ.deleteResult(e).then((data) => {
      this.dataSource.next(data)
    })
  }
  public generateNotebook(){
    const cnd = this.condition_id.split(" ")
    const pID = this.participant_id
    this.resServ.createJuypterNotebook(this.experimentID,pID,cnd)
  }
  public openNotebook(){
    if(environment.devEnvironment){
      window.open(`https://jupyter.kai-biermeier.de/lab/tree/artos/experiments/experiment${this.experimentID}/notebook.ipynb`, '_blank');
    }
    else {
      window.open(`https://jupyter.${environment.domain}/lab/tree/artos/experiments/experiment${this.experimentID}/notebook.ipynb`, '_blank');
    }
  }

  ngOnInit(): void {
    this.participantServ.getParticipantID(this.experimentID).then((participantID) => {
      this.currentParticipant = participantID;
    })
  }

}
