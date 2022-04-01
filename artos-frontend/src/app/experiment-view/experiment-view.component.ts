import { ExperimentalResult } from './../resultService/ExperimentalResult';
import { BehaviorSubject } from 'rxjs';
import { ResultService } from './../resultService/result.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-experiment-view',
  templateUrl: './experiment-view.component.html',
  styleUrls: ['./experiment-view.component.scss']
})
export class ExperimentViewComponent implements OnInit {
  displayedColumns: string[] = ['participant_id','timestamp','result','delete'];
  dataSource = new BehaviorSubject<ExperimentalResult[]>([{participant_id:0,result:"a",timestamp:"b"}])

  constructor(private resServ: ResultService) {
    resServ.getResults().then((data) => {
      this.dataSource.next(data)
    })
  }

  public deleteElement(e:ExperimentalResult){
    this.resServ.deleteResult(e).then((data) => {
      this.dataSource.next(data)
    })
  }

  ngOnInit(): void {
  }

}
