export enum TaskStatus {
  Pending = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4
}

export interface TaskItem {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  dueDate: string;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  dueDate: string;
}

export interface UpdateTaskRequest {
  title: string;
  description: string;
  dueDate: string;
}

export interface ChangeTaskStatusRequest {
  status: TaskStatus;
}
