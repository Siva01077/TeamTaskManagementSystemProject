import { User } from './user.model';

export interface TaskItem {
  id: number;
  title: string;
  description: string;
  status: 'To Do' | 'In Progress' | 'Done';
  priority: 'Low' | 'Medium' | 'High';
  deadline: string;
  assignedToUserId: number;
  assignedToUser?: User;
  teamId?: number | null;
  createdByUserId: number;
  createdAt: string;
  comments?: CommentItem[];
}

export interface CommentItem {
  id: number;
  taskId: number;
  userId: number;
  user?: User;
  content: string;
  createdAt: string;
}
