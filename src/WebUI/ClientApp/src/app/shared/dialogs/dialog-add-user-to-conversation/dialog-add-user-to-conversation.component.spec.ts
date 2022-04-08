import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogAddUserToConversationComponent } from './dialog-add-user-to-conversation.component';

describe('DialogAddUserToConversationComponent', () => {
  let component: DialogAddUserToConversationComponent;
  let fixture: ComponentFixture<DialogAddUserToConversationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogAddUserToConversationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogAddUserToConversationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
