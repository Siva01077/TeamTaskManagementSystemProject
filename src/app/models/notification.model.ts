export interface NotificationItem {
  id: number;
  userId: number;
  taskId?: number | null;
  type: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}
