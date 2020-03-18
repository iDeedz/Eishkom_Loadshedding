CREATE TABLE [dbo].[Eskom_LoadsheddingStatus] (
    [Id]        INT      IDENTITY (1, 1) NOT NULL,
    [Timestamp] DATETIME NULL,
    [Stage]     INT      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

