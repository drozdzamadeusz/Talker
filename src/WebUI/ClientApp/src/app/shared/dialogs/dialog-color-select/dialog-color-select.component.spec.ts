import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogColorSelectComponent } from './dialog-color-select.component';

describe('DialogColorSelectComponent', () => {
  let component: DialogColorSelectComponent;
  let fixture: ComponentFixture<DialogColorSelectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogColorSelectComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogColorSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
