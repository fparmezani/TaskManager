# GenAI Code Generation Rules

This project allows AI assistance, but AI output must be reviewed critically before being accepted.

## Preferred Tool

Codex, Claude Code, Cursor, GitHub Copilot, or another GenAI coding assistant may be used.

## Main Prompt

```text
Generate a .NET 8 Clean Architecture REST API for a task management system.

Requirements:
- Use ASP.NET Core Web API.
- Use SQL Server.
- Do not use Entity Framework.
- Do not use Dapper.
- Do not use Mediator or MediatR.
- Use ADO.NET with Microsoft.Data.SqlClient for data access.
- Implement JWT authentication.
- Implement user registration and login.
- Implement CRUD endpoints for tasks.
- Each task must have title, description, status, due date, and user id.
- Use repository interfaces in the Application layer.
- Implement repositories in the Infrastructure layer.
- Keep Domain independent from external dependencies.
- Use xUnit, FluentAssertions, and NSubstitute for tests.
- Include Docker Compose with SQL Server.
- Include seeded demo user and seeded tasks.
- Follow Clean Code, SOLID, and secure coding practices.
```

## Rules for Accepting AI Output

1. The Domain project must not reference Infrastructure or API.
2. Repository interfaces must be in Application.
3. SQL implementation must be in Infrastructure.
4. Controllers must not contain business rules.
5. Passwords must never be stored as plain text.
6. SQL commands must use parameters.
7. JWT secret must come from configuration.
8. Task queries must be scoped by authenticated user id.
9. No `SELECT *` in repositories.
10. Tests must verify business behavior, not implementation details only.

## Typical AI Corrections

AI tools often generate code that needs correction:

- Replacing Entity Framework with ADO.NET.
- Removing MediatR/CQRS when not needed.
- Moving validation from controllers to Domain/Application.
- Adding SQL parameters to avoid SQL injection.
- Adding user scoping to prevent accessing another user's tasks.
- Adding cancellation tokens.
- Replacing plain-text passwords with BCrypt.
- Adding seeded data for demo.
