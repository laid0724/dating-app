export type Role = 'Admin' | 'Moderator' | 'Member';
export interface User {
  userName: string;
  token: string;
  photoUrl: string;
  knownAs: string;
  gender: string;
  roles: Role[];
}
