import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogConversationUsersComponent } from './dialog-conversation-users.component';

describe('DialogConversationUsersComponent', () => {
  let component: DialogConversationUsersComponent;
  let fixture: ComponentFixture<DialogConversationUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogConversationUsersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogConversationUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
