# Presentation Notes

## Project Summary

This project is a task management system built for a .NET technical interview exercise. It includes a REST API, authentication, SQL Server persistence, seeded demo data, tests, Docker setup, and Clean Architecture.

## User Story

As a user, I want to manage my personal tasks so that I can track my work and deadlines efficiently.

## Main Design Choice

Because Entity Framework, Dapper, and Mediator were not allowed, the project uses ADO.NET directly through `Microsoft.Data.SqlClient`.

## Why ADO.NET

ADO.NET demonstrates direct control over:

- SQL commands.
- SQL parameters.
- Database connections.
- Manual mapping.
- Transactions when needed.

## Why No CQRS/Mediator

The assignment is CRUD-focused. A service-oriented flow is simpler, easier to review, and still compatible with Clean Architecture.

```text
Controller -> Application Service -> Repository Interface -> SQL Repository -> SQL Server
```

## Demo Flow

1. Open Swagger.
2. Call public endpoint.
3. Login with demo credentials.
4. Authorize Swagger with Bearer token.
5. List tasks.
6. Create task.
7. Update task.
8. Change status.
9. Delete task.
10. Show tests.
11. Show Docker Compose and seed script.

## AI Usage Explanation

AI was used to scaffold and accelerate development, but the code was manually validated for architecture, security, SQL injection prevention, authentication flow, testability, and assignment restrictions.
