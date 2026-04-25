IF DB_ID('TaskManagerDb') IS NULL
BEGIN
    CREATE DATABASE TaskManagerDb;
END
GO

USE TaskManagerDb;
GO

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Email NVARCHAR(256) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(500) NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.Tasks', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tasks (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        UserId UNIQUEIDENTIFIER NOT NULL,
        Title NVARCHAR(120) NOT NULL,
        Description NVARCHAR(1000) NOT NULL,
        Status INT NOT NULL,
        DueDate DATETIME2 NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL,
        UpdatedAtUtc DATETIME2 NULL,
        CONSTRAINT FK_Tasks_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'demo@taskmanager.com')
BEGIN
    DECLARE @DemoUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';

    INSERT INTO dbo.Users (Id, Email, PasswordHash, CreatedAtUtc)
    VALUES (@DemoUserId, 'demo@taskmanager.com', 'PBKDF2-SHA256$10000$dGFza21hbmFnZXItZGVtby1zYWx0LTIwMjY=$kD0KOici0orLRHViNe1AGvpH11+JgHPW3x7suuvZLbg=', SYSUTCDATETIME());

    INSERT INTO dbo.Tasks (Id, UserId, Title, Description, Status, DueDate, CreatedAtUtc, UpdatedAtUtc)
    VALUES
    (NEWID(), @DemoUserId, 'Finish technical interview project', 'Complete API, frontend, tests, Docker setup and README.', 2, DATEADD(day, 7, SYSUTCDATETIME()), SYSUTCDATETIME(), NULL),
    (NEWID(), @DemoUserId, 'Review Clean Architecture presentation', 'Prepare a concise explanation of layers and dependencies.', 1, DATEADD(day, 10, SYSUTCDATETIME()), SYSUTCDATETIME(), NULL),
    (NEWID(), @DemoUserId, 'Prepare GenAI prompt engineering explanation', 'Document validation, corrections and edge cases handled after AI output.', 1, DATEADD(day, 14, SYSUTCDATETIME()), SYSUTCDATETIME(), NULL);
END
GO
