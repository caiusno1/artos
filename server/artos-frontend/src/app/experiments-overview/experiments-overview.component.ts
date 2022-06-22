import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-experiments-overview',
  templateUrl: './experiments-overview.component.html',
  styleUrls: ['./experiments-overview.component.scss']
})
export class ExperimentsOverviewComponent implements OnInit {

  public experiments: number[] = [1]
  constructor() { }

  ngOnInit(): void {
  }

}
