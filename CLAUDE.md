# CLAUDE.md - TaskManager Project Documentation Index

> **Project**: TaskManager - .NET 8 Clean Architecture REST API  
> **Purpose**: Technical interview exercise - Task Management System  
> **Last Updated**: April 2026

---

## 📁 Project Structure

```
TaskManager/
├── backend/                      # .NET 8 Web API (Clean Architecture)
│   ├── TaskManager.Api/          # API layer - Controllers, Middleware
│   ├── TaskManager.Application/  # Application layer - Services, DTOs, Interfaces
│   ├── TaskManager.Domain/       # Domain layer - Entities, Enums, Exceptions
│   └── TaskManager.Infrastructure/ # Infrastructure - Repositories, Data, Security
├── frontend/                     # Angular 17+ standalone components
│   └── taskmanager-web/
├── docker/                       # Docker configuration files
│   ├── backend/
│   │   └── Dockerfile            # .NET 8 API container
│   ├── frontend/
│   │   └── Dockerfile            # Angular 17+ container
│   └── README.md                 # Docker documentation
├── tests/                        # Test projects (Unit, Integration, API)
├── database/                     # SQL Server initialization scripts
└── docs/                         # Documentation files (see below)
```

---

## 📚 Documentation Files

All project documentation is stored in the [`docs/`](docs/) folder:

### 1. [code-generation.md](docs/code-generation.md)
**Purpose**: GenAI Code Generation Rules  
**Contains**:
- Preferred AI tools (Codex, Claude Code, Cursor, GitHub Copilot)
- Main prompt for generating .NET 8 Clean Architecture API
- Rules for accepting AI output (10 key architectural constraints)
- Typical AI corrections and validation checklist

**Key Constraints**:
- ❌ No Entity Framework
- ❌ No Dapper
- ❌ No Mediator/MediatR
- ✅ ADO.NET with `Microsoft.Data.SqlClient`
- ✅ Repository pattern with interfaces in Application layer
- ✅ SQL implementation in Infrastructure layer

---

### 2. [frontend.md](docs/frontend.md)
**Purpose**: Frontend Implementation Notes  
**Contains**:
- Angular standalone components architecture
- Reactive Forms for authentication and task management
- Functional guards and interceptors
- Tailwind CSS for responsive UI
- Route structure (`/login`, `/register`, `/tasks`)
- API integration and proxy configuration
- Demo credentials

**Tech Stack**:
- Angular 17+ (standalone components)
- Tailwind CSS
- RxJS for reactive programming
- JWT authentication via interceptors

---

### 3. [presentation.md](docs/presentation.md)
**Purpose**: Presentation Notes for Demo/Interview  
**Contains**:
- Project summary and user story
- Design decisions rationale (ADO.NET, no CQRS/Mediator)
- Architecture flow diagram
- Demo flow (11 steps from Swagger to Docker)
- AI usage explanation and validation approach

**Architecture Flow**:
```
Controller → Application Service → Repository Interface → SQL Repository → SQL Server
```

---

### 4. [review.md](docs/review.md)
**Purpose**: Code Review Checklist  
**Contains**:
- Clean Architecture validation points
- Security checklist (passwords, JWT, SQL injection)
- Data access rules (ADO.NET, no ORM)
- API design standards (HTTP verbs, status codes)
- Test coverage expectations
- Browser/Frontend quality criteria

**Review Focus Areas**:
- ✅ Domain independence
- ✅ Password hashing (PBKDF2-SHA256)
- ✅ JWT validation (issuer, audience, lifetime, signing key)
- ✅ User data isolation
- ✅ SQL parameterization

---

### 5. [security.md](docs/security.md)
**Purpose**: Security Implementation Notes  
**Contains**:
- Password storage strategy (PBKDF2-SHA256 with random salt)
- Demo user credentials and hash
- JWT configuration requirements
- SQL injection prevention measures
- Production deployment security recommendations

**Security Highlights**:
- 🔒 Passwords never stored in plain text
- 🔒 JWT secret from configuration (environment variables)
- 🔒 All SQL commands use parameters
- 🔒 User-scoped task queries

---

### 6. [test-plan.md](docs/test-plan.md)
**Purpose**: Test Strategy Overview  
**Contains**:
- Test project structure (Unit, Integration, API tests)
- Layer-specific test responsibilities
- Domain and Application test coverage
- Integration test setup with Testcontainers
- API test authentication flows

**Test Pyramid**:
```
        API Tests (HTTP, JWT, Authorization)
           /
Integration Tests (SQL Server, ADO.NET)
         /
  Unit Tests (Domain, Application)
```

---

### 7. [testing.md](docs/testing.md)
**Purpose**: Testing Guidelines and Best Practices  
**Contains**:
- Test organization by responsibility
- Naming pattern for test methods
- Unit test rules (no database, no HTTP, no Docker)
- Integration test rules (Testcontainers, real SQL Server)
- API test rules (`WebApplicationFactory<Program>`)

**Naming Convention**:
```
MethodName_Should_ExpectedBehavior_When_Condition
```

**Example**:
```
CreateAsync_Should_Create_Task_When_Request_Is_Valid
LoginAsync_Should_Fail_When_Password_Is_Invalid
```

---

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- Node.js 18+ (for frontend)
- SQL Server (via Docker Compose)

### Running the Project

```bash
# Start all services (API, Frontend, SQL Server)
docker-compose up --build

# Access points:
# - API + Swagger: http://localhost:8080
# - Frontend: http://localhost:4200
# - SQL Server: localhost:14333
```

### Docker Structure

All Docker configuration is now organized in the [`docker/`](docker/) folder:
- `docker/backend/Dockerfile` - .NET 8 API container
- `docker/frontend/Dockerfile` - Angular 17+ container
- `docker/README.md` - Complete Docker documentation

### Demo Credentials
```
Email: demo@taskmanager.com
Password: Demo@123456
```

---

## 🎯 Key Technologies

### Backend
- .NET 8 Web API
- ADO.NET (`Microsoft.Data.SqlClient`)
- SQL Server
- JWT Authentication
- PBKDF2-SHA256 Password Hashing
- Clean Architecture

### Frontend
- Angular 17+ (Standalone Components)
- TypeScript
- Tailwind CSS
- RxJS
- Reactive Forms

### Testing
- xUnit
- FluentAssertions
- NSubstitute
- Testcontainers (for integration tests)
- `WebApplicationFactory<T>` (for API tests)

---

## 📋 Assignment Constraints

This project was built following specific technical interview constraints:

| ❌ Not Allowed | ✅ Used Instead |
|---------------|----------------|
| Entity Framework | ADO.NET |
| Dapper | `Microsoft.Data.SqlClient` |
| Mediator/MediatR | Service-oriented architecture |
| Module-based Angular | Standalone components |

---

## 🤖 AI Usage Policy

AI assistance is encouraged but must be critically reviewed:

1. **Architecture validation**: Ensure Clean Architecture layers remain independent
2. **Security review**: Verify SQL parameterization, password hashing, JWT configuration
3. **Test coverage**: Confirm business logic is tested, not just implementation details
4. **Code quality**: Apply SOLID principles and Clean Code practices

---

## 📞 Contact & Support

For questions about this project or documentation, refer to the specific `.md` file in the [`docs/`](docs/) folder that matches your topic.

**Documentation maintained by**: Development Team  
**AI Assistant Reference**: This file serves as the main index for Claude and other AI assistants working with this codebase.
