# Test Plan

This project uses a layered test strategy aligned with Clean Architecture and TDD.

## Test Projects

```text
tests/
├── TaskManager.UnitTests
│   ├── Domain
│   └── Application
├── TaskManager.IntegrationTests
│   ├── Database
│   ├── Repositories
│   └── Support
└── TaskManager.ApiTests
    ├── Auth
    ├── Tasks
    ├── Authorization
    └── Support
```

## 1. Unit Tests

Unit tests are fast and isolated. They do not use database, HTTP, Docker, or external services.

### Domain

Covered classes:

- `TaskItem`
- `User`

Covered scenarios:

- Successful entity creation
- Required fields
- Maximum length validation
- Due date validation
- User id validation
- Status changes
- Update behavior

### Application

Covered services:

- `TaskService`
- `UserService`

Covered scenarios:

- Successful task creation
- Failed task creation by invalid domain data
- Get task list by user
- Get task by id not found
- Update success
- Update not found
- Change status success
- Delete delegates to repository
- Register success
- Register duplicated email
- Register short password
- Login success
- Login invalid user
- Login invalid password

## 2. Integration Tests

Integration tests validate real infrastructure behavior using SQL Server with Testcontainers.

Covered components:

- `DatabaseInitializer`
- `SqlUserRepository`
- `SqlTaskRepository`

Covered scenarios:

- Database initialization
- Users table creation
- Tasks table creation
- Demo user seed
- Seed idempotency
- User insert and read
- User not found
- Task insert
- Task read by id
- Task list by user
- Task update
- Task delete
- Task isolation by user

## 3. API Tests

API tests validate real HTTP behavior using `WebApplicationFactory` plus SQL Server Testcontainers.

Covered endpoint groups:

- Auth endpoints
- Task CRUD endpoints
- Validation endpoints
- Authorization endpoints

Covered scenarios:

- Register success
- Register duplicate email
- Register short password
- Login success
- Login wrong password
- Login missing user
- Create task
- List tasks
- Get task by id
- Update task
- Change task status
- Delete task
- Invalid task title
- Invalid due date

## 4. Authorization Tests

Authorization tests are separated because they are business-critical.

Covered scenarios:

- Protected endpoint without token returns `401 Unauthorized`
- Public endpoint without token returns `200 OK`
- User B cannot read User A task
- User B cannot update User A task
- User B cannot delete User A task data

## TDD Approach

The intended TDD cycle is:

```text
Red -> Green -> Refactor
```

For each feature:

1. Write the failing unit test first.
2. Implement the smallest amount of production code.
3. Refactor while keeping tests green.
4. Add integration/API tests for persistence and HTTP behavior.

## Coverage Goal

Target coverage:

- Domain: 95%+
- Application: 90%+
- Infrastructure: main repository/database flows covered
- API: critical endpoint flows covered
- Authorization: all protected task endpoints covered

## Test Commands

Run all tests:

```bash
dotnet test
```

Run unit tests only:

```bash
dotnet test tests/TaskManager.UnitTests/TaskManager.UnitTests.csproj
```

Run integration tests only:

```bash
dotnet test tests/TaskManager.IntegrationTests/TaskManager.IntegrationTests.csproj
```

Run API tests only:

```bash
dotnet test tests/TaskManager.ApiTests/TaskManager.ApiTests.csproj
```

Run tests with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```
