DECLARE @DbName NVarChar(50) = (SELECT DB_NAME())

IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'RepoStats')
    BEGIN
        EXEC sp_executesql N'CREATE SCHEMA [RepoStats]'
    END

IF object_id('[RepoStats].[Repos]') IS NULL 
    BEGIN
        CREATE TABLE [RepoStats].[Repos]
            (
            [RepoId]             uniqueidentifier NOT NULL PRIMARY KEY,
            [RepoUrl]            NVARCHAR(255) NOT NULL,
            [ParseStatus]        INT NOT NULL
            )
    END

IF object_id('[RepoStats].[Repo_Languages]') IS NULL 
    BEGIN
        CREATE TABLE [RepoStats].[Repo_Languages]
            (
            [Id]                 INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            [RepoId]             NVARCHAR(40) NOT NULL,
            [Language]           NVARCHAR(30) NOT NULL,
            [Bytes]              BIGINT NOT NULL
            )
    END

IF object_id('[RepoStats].[Repo_Contributors]') IS NULL 
    BEGIN
        CREATE TABLE [RepoStats].Repo_Contributors
            (
            [Id]                 INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            [RepoId]             NVARCHAR(40) NOT NULL,
            [UserName]           NVARCHAR(255) NOT NULL,
            [Contributions]      INT NOT NULL
            )
    END
