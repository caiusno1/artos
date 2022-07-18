import { Experiment } from './../experimentService/Experiment';
import { ExperimentService } from './../experimentService/experiment.service';
import { DeleteDialogData } from './DeleteDialogData';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-delete-confirmation',
  templateUrl: './delete-confirmation.component.html',
  styleUrls: ['./delete-confirmation.component.scss']
})
export class DeleteConfirmationComponent implements OnInit {

  constructor(private dialogRef: MatDialogRef<DeleteConfirmationComponent>,
    @Inject(MAT_DIALOG_DATA) private data: DeleteDialogData,private expServ: ExperimentService) { }

  ngOnInit(): void {
  }
  public async deleteXP(){
    const exp = this.data.itemToDelete
    await this.expServ.deleteExperiment(exp.ID)
    this.dialogRef.close(exp.ID)
  }
  public abort(){
    this.dialogRef.close(undefined)
  }

}
