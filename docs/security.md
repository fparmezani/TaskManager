# Security Notes

## Password Storage

Passwords are hashed using PBKDF2-SHA256 with random salt for newly registered users.

The seeded demo user uses a precomputed PBKDF2-SHA256 hash for:

```text
Demo@123456
```

## JWT

JWT settings are loaded from configuration:

- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Secret`

For real production deployment, replace the demo secret with a secret from a secure vault or environment variable.

## SQL Injection Prevention

All repository commands use SQL parameters. No user input should be concatenated into SQL strings.
