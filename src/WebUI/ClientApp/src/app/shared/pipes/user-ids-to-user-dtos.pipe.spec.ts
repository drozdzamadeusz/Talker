import { UserIdsToUserDtosPipe } from './user-ids-to-user-dtos.pipe';

describe('UserIdsToUserDtosPipe', () => {
  it('create an instance', () => {
    const pipe = new UserIdsToUserDtosPipe();
    expect(pipe).toBeTruthy();
  });
});
