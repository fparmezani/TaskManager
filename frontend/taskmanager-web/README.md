# TaskManager Web

Angular frontend for the TaskManager interview project.

## Stack

- Angular standalone components
- Reactive Forms
- Functional guards and interceptors
- Tailwind CSS
- JWT Bearer authentication

## Local development

```bash
cd frontend/taskmanager-web
npm install
npm start
```

The local Angular dev server uses `proxy.conf.json` to forward `/api` requests to the .NET API running on `http://localhost:8080`.

## Docker

From the repository root:

```bash
docker compose up --build
```

Frontend URL:

```text
http://localhost:4200
```

Demo credentials:

```text
Email: demo@taskmanager.com
Password: Demo@123456
```

## Frontend structure

```text
src/app
 ├── core
 │   ├── guards
 │   ├── interceptors
 │   ├── models
 │   └── services
 ├── features
 │   ├── auth
 │   └── tasks
 └── shared
```
