# TaskManager Interview Project

A .NET 8 technical interview exercise implementing a task management system with Clean Architecture, TDD, JWT authentication, Docker, SQL Server, and a Angular frontend.

## User Story

As a user, I want to manage my personal tasks so that I can track my work and deadlines efficiently.

## Technical Decisions

- No Entity Framework.
- No Dapper.
- No Mediator/MediatR.
- Data access is implemented with ADO.NET using `Microsoft.Data.SqlClient`.
- Clean Architecture is used to separate API, Application, Domain, and Infrastructure concerns.
- SQL Server runs in Docker.
- Demo data and credentials are seeded automatically.

## Repository Layout

```text
TaskManager/
 ├── backend/                     # .NET backend projects
 │   ├── TaskManager.Api/         # Controllers, Middleware
 │   ├── TaskManager.Application/ # Services, DTOs, Interfaces
 │   ├── TaskManager.Domain/      # Entities, Enums, Exceptions
 │   └── TaskManager.Infrastructure/ # Repositories, Security, Data
 ├── tests/                       # Unit, Integration, and API tests
 ├── frontend/taskmanager-web/    # Angular frontend
 ├── database/                    # SQL Server init and seed scripts
 ├── docker/                      # Dockerfiles for API and frontend
 ├── docs/                        # GenAI, review, testing, and presentation notes
 └── docker-compose.yml           # SQL Server + API + Angular + Ollama
```

## Architecture

```text
TaskManager.Api
  -> Controllers, authentication, HTTP concerns

TaskManager.Application
  -> Use cases, services, DTOs, repository contracts

TaskManager.Domain
  -> Entities, enums, domain validation

TaskManager.Infrastructure
  -> SQL Server repositories, JWT, password hashing
```

Dependency direction:

```text
Api -> Application -> Domain
Infrastructure -> Application + Domain
```

## Demo Credentials

```text
Email: demo@taskmanager.com
Password: Demo@123456
```

## Run with Docker

```bash
docker compose up --build
```

API:

```text
http://localhost:8080
```

Frontend:

```text
http://localhost:4200
```

Swagger:

```text
http://localhost:8080/swagger
```

SQL Server:

```text
localhost,14333
User: sa
Password: Your_strong_password123
Database: TaskManagerDb
```

## Main Endpoints

### Auth

```http
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/public
GET  /api/auth/secure
```

### Tasks

```http
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
PATCH  /api/tasks/{id}/status
DELETE /api/tasks/{id}
```

Task endpoints require JWT Bearer authentication.

## Run Tests

```bash
dotnet test

# or run by category
dotnet test tests/TaskManager.UnitTests/TaskManager.UnitTests.csproj
dotnet test tests/TaskManager.IntegrationTests/TaskManager.IntegrationTests.csproj
dotnet test tests/TaskManager.ApiTests/TaskManager.ApiTests.csproj
```

## GenAI Requirement

See:

- `docs/prompt-history.md` — histórico completo de prompts, outputs, validações e correções
- `docs/code-generation.md` — regras de aceitação de código gerado por AI
- `docs/review.md` — checklist de revisão
- `docs/presentation.md` — notas para a apresentação

These files document the AI prompt strategy, validation process, corrections, edge cases, and review checklist.
