import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotepadWindowComponent } from './notepad-window.component';

describe('NotepadWindowComponent', () => {
  let component: NotepadWindowComponent;
  let fixture: ComponentFixture<NotepadWindowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NotepadWindowComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NotepadWindowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
