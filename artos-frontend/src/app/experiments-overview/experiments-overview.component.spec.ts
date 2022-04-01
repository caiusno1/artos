import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExperimentsOverviewComponent } from './experiments-overview.component';

describe('ExperimentsOverviewComponent', () => {
  let component: ExperimentsOverviewComponent;
  let fixture: ComponentFixture<ExperimentsOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExperimentsOverviewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ExperimentsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
