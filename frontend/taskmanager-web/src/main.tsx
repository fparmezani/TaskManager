import React, { useEffect, useState } from 'react';
import { createRoot } from 'react-dom/client';
import axios from 'axios';
import './styles.css';

type TaskStatus = 1 | 2 | 3 | 4;

type TaskItem = {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  dueDate: string;
};

const api = axios.create({ baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:8080' });

function App() {
  const [token, setToken] = useState(localStorage.getItem('accessToken') ?? '');
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [dueDate, setDueDate] = useState('');
  const [error, setError] = useState('');

  async function login() {
    setError('');
    const response = await api.post('/api/auth/login', {
      email: 'demo@taskmanager.com',
      password: 'Demo@123456'
    });

    localStorage.setItem('accessToken', response.data.accessToken);
    setToken(response.data.accessToken);
  }

  async function loadTasks(currentToken = token) {
    if (!currentToken) return;
    const response = await api.get<TaskItem[]>('/api/tasks', {
      headers: { Authorization: `Bearer ${currentToken}` }
    });
    setTasks(response.data);
  }

  async function createTask(event: React.FormEvent) {
    event.preventDefault();
    setError('');

    await api.post('/api/tasks', {
      title,
      description,
      dueDate: new Date(dueDate).toISOString()
    }, {
      headers: { Authorization: `Bearer ${token}` }
    });

    setTitle('');
    setDescription('');
    setDueDate('');
    await loadTasks();
  }

  async function removeTask(id: string) {
    await api.delete(`/api/tasks/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    });
    await loadTasks();
  }

  useEffect(() => {
    loadTasks().catch(() => setError('Unable to load tasks. Please login again.'));
  }, []);

  return (
    <main className="page">
      <section className="card">
        <h1>TaskManager</h1>
        <p>Clean Architecture .NET API + React frontend demo.</p>
        {!token && <button onClick={login}>Login with demo user</button>}
        {token && <button onClick={() => loadTasks()}>Refresh tasks</button>}
        {error && <p className="error">{error}</p>}
      </section>

      <section className="grid">
        <form className="card" onSubmit={createTask}>
          <h2>Create Task</h2>
          <input value={title} onChange={e => setTitle(e.target.value)} placeholder="Title" required />
          <textarea value={description} onChange={e => setDescription(e.target.value)} placeholder="Description" required />
          <input type="date" value={dueDate} onChange={e => setDueDate(e.target.value)} required />
          <button disabled={!token}>Create</button>
        </form>

        <section className="card">
          <h2>Tasks</h2>
          {tasks.map(task => (
            <article className="task" key={task.id}>
              <strong>{task.title}</strong>
              <span>{task.description}</span>
              <small>Due: {new Date(task.dueDate).toLocaleDateString()}</small>
              <button onClick={() => removeTask(task.id)}>Delete</button>
            </article>
          ))}
        </section>
      </section>
    </main>
  );
}

createRoot(document.getElementById('root')!).render(<App />);
