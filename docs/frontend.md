# Frontend Implementation Notes

The frontend is implemented with Angular and kept inside the same repository as the backend.

## Design choices

- Angular standalone components to reduce module boilerplate.
- Reactive Forms for login, register, and task management.
- Functional `authGuard` to protect task routes.
- Functional `authInterceptor` to attach the JWT token to API calls.
- Tailwind CSS for responsive UI without heavy component dependencies.
- API access is centralized in services under `core/services`.

## Routes

```text
/login      Public login page
/register   Public registration page
/tasks      Protected task CRUD page
```

## API integration

The frontend calls `/api`. In Docker, Nginx proxies `/api` to the backend container. In local development, Angular uses `proxy.conf.json` to forward `/api` to `http://localhost:8080`.

## Demo credentials

```text
Email: demo@taskmanager.com
Password: Demo@123456
```
