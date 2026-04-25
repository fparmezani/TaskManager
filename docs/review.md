# Code Review Checklist

## Clean Architecture

- Domain has no dependency on API, Infrastructure, or Application implementation details.
- Application defines use cases and contracts.
- Infrastructure implements repository contracts.
- API only handles HTTP, routing, authentication, and response mapping.

## Security

- Passwords are hashed using BCrypt.
- JWT is validated with issuer, audience, lifetime, and signing key.
- Protected task endpoints require authentication.
- Users can only access their own tasks.
- SQL uses parameters only.
- No secrets should be committed for production.

## Data Access

- No Entity Framework.
- No Dapper.
- No Mediator/MediatR.
- ADO.NET is used directly through `Microsoft.Data.SqlClient`.
- SQL statements list explicit columns.
- Repositories are small and focused.

## API Design

- HTTP verbs match intent.
- `POST` returns `201 Created`.
- `GET` returns `200 OK` or `404 Not Found`.
- `PUT` updates a full resource.
- `PATCH` changes task status.
- `DELETE` returns `204 No Content`.

## Tests

- Domain validation is tested.
- Application services are tested with mocked repositories.
- Authentication paths should be covered.
- Repository integration tests can be added with Testcontainers.

## Browser / Frontend Expectations

- No browser console warnings.
- API errors should be handled cleanly.
- UI should be responsive.
- Token should be stored carefully.
