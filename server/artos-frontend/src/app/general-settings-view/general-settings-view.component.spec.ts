import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GeneralSettingsViewComponent } from './general-settings-view.component';

describe('GeneralSettingsViewComponent', () => {
  let component: GeneralSettingsViewComponent;
  let fixture: ComponentFixture<GeneralSettingsViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GeneralSettingsViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GeneralSettingsViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
