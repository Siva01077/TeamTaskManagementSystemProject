export interface User {
  id: number;
  fullName: string;
  email: string;
  role: 'Admin' | 'Manager' | 'User';
  teamId?: number | null;
  team?: Team | null;
}

export interface Team {
  id: number;
  name: string;
  users?: User[];
}
