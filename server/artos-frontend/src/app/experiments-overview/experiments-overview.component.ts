import { DeleteConfirmationComponent } from './../delete-confirmation/delete-confirmation.component';
import { ExperimentService } from './../experimentService/experiment.service';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Experiment } from '../experimentService/Experiment';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-experiments-overview',
  templateUrl: './experiments-overview.component.html',
  styleUrls: ['./experiments-overview.component.scss']
})
export class ExperimentsOverviewComponent implements OnInit{

  public experiments: Experiment[] = []
  public currentView: number = -1;
  constructor(private expServ: ExperimentService, private dialog: MatDialog) {

  }

  createNewExperiment(){
    this.expServ.createNewExperiment().then(async () => {
      this.experiments = await this.expServ.getExperiments();
      if(this.experiments.length > 0){
        this.currentView = 0;
      }
    })
  }
  tabChanged(tabView: MatTabChangeEvent){
    this.currentView = tabView.index
  }
  public async deleteExperiment(){
    if(this.currentView >= 0){
      console.log(this.experiments[this.currentView].ID)
      let dialogRef = this.dialog.open(DeleteConfirmationComponent, {
        height: '170px',
        width: '400px',
        data:{itemToDelete: this.experiments[this.currentView]}
      });
      dialogRef.afterClosed().subscribe(async result => {
        if(result){
          this.experiments = await this.expServ.getExperiments()
          if(this.experiments.length == 0){
            this.currentView = -1;
          }
        }
      });

    }
  }

  async ngOnInit() {
    this.experiments = await this.expServ.getExperiments();
  }
}
