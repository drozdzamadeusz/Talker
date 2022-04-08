import { TestBed } from '@angular/core/testing';

import { UserClientCacheService } from './user-client-cache.service';

describe('UserClientCacheService', () => {
  let service: UserClientCacheService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserClientCacheService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
