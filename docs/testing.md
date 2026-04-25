# Testing Guidelines

## Organization

Tests are organized by responsibility:

```text
TaskManager.UnitTests          Fast isolated tests
TaskManager.IntegrationTests   SQL Server, ADO.NET, database initialization
TaskManager.ApiTests           HTTP, JWT authentication, authorization, endpoint behavior
```

## Naming Pattern

Use this naming pattern:

```text
MethodName_Should_ExpectedBehavior_When_Condition
```

Examples:

```text
CreateAsync_Should_Create_Task_When_Request_Is_Valid
LoginAsync_Should_Fail_When_Password_Is_Invalid
Get_Tasks_Should_Return_Unauthorized_When_No_Token
```

## Unit Test Rules

- No database.
- No HTTP.
- No Docker.
- Use mocks/substitutes for repository interfaces.
- Validate both success and failure paths.
- Keep each test focused on one behavior.

## Integration Test Rules

- Use a real SQL Server container through Testcontainers.
- Validate SQL scripts, ADO.NET mapping, persistence, updates, deletes, and seed idempotency.
- Use unique test data to avoid cross-test collisions.

## API Test Rules

- Use `WebApplicationFactory<Program>`.
- Use a real SQL Server container.
- Validate HTTP status codes and response bodies.
- Test authenticated and unauthenticated flows.
- Test user data isolation.

## Authorization Rules

Every protected endpoint should have a matching unauthorized test.

Protected endpoints:

```text
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
PATCH  /api/tasks/{id}/status
DELETE /api/tasks/{id}
GET    /api/auth/secure
```

## Edge Cases to Keep Covered

- Empty title
- Title too long
- Empty description
- Description too long
- Due date in the past
- Duplicate email
- Short password
- Missing user
- Invalid password
- User trying to access another user's task
