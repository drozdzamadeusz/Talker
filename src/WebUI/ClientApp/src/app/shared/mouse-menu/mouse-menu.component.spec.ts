import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MouseMenuComponent } from './mouse-menu.component';

describe('MouseMenuComponent', () => {
  let component: MouseMenuComponent;
  let fixture: ComponentFixture<MouseMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MouseMenuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MouseMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
