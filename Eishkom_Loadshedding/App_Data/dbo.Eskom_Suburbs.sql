CREATE TABLE [dbo].[Eskom_Suburbs] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [SuburbID]       INT           NULL,
    [MunicipalityID] INT           NULL,
    [Description]    VARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

