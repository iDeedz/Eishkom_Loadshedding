CREATE TABLE [dbo].[Eskom_Provinces] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (50) NULL,
    [ProvinceID]  INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

