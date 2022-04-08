import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogMessageDetails } from './dialog-message-details.component';

describe('DialogMessageDetailsComponent', () => {
  let component: DialogMessageDetails;
  let fixture: ComponentFixture<DialogMessageDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogMessageDetails ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogMessageDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
