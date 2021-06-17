
export interface Photo {
  id: number;
  url: string;
  isMain: boolean;
  isApproved: boolean;
}

export interface PhotoForApproval {
  id: number;
  url: string;
  userName: string;
  isApproved: boolean;
}
