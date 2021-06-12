import { PagedRequest } from './pagination';
import { User } from './users';

export class UserParams implements PagedRequest {
  gender: string;
  minAge = 18;
  maxAge = 99;
  pageNumber = 1;
  pageSize = 6;
  orderBy: 'created' | 'lastActive' = 'lastActive';

  constructor(user: User) {
    this.gender = user.gender === 'male' ? 'female' : 'male';
  }
}
