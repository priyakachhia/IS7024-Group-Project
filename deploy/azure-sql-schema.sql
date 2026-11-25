SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.HistoryEntries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HistoryEntries
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_HistoryEntries PRIMARY KEY,
        Neighborhood nvarchar(max) NOT NULL,
        BusinessType nvarchar(max) NOT NULL,
        ReportedCrimes int NOT NULL,
        BuildingPermits int NOT NULL,
        SafetyScore float NOT NULL,
        GrowthScore float NOT NULL,
        OverallScore int NOT NULL,
        SavedAtUtc datetime2 NOT NULL
    );
END;

IF OBJECT_ID(N'dbo.CommunityPosts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CommunityPosts
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CommunityPosts PRIMARY KEY,
        Title nvarchar(120) NOT NULL,
        Neighborhood nvarchar(100) NOT NULL,
        BusinessType nvarchar(50) NOT NULL,
        Category nvarchar(50) NOT NULL,
        Message nvarchar(2000) NOT NULL,
        CreatedAtUtc datetime2 NOT NULL
    );
END;

COMMIT TRANSACTION;
