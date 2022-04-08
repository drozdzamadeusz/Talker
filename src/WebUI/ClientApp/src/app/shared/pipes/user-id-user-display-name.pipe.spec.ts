import { UserIdUserDisplayNamePipe } from './user-id-user-display-name.pipe';

describe('UserIdUserDisplayNamePipe', () => {
  it('create an instance', () => {
    const pipe = new UserIdUserDisplayNamePipe();
    expect(pipe).toBeTruthy();
  });
});
