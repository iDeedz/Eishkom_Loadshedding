CREATE TABLE [dbo].[Eskom_Municipalities] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [ProvinceID]     INT           NULL,
    [MunicipalityID] INT           NULL,
    [Description]    VARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

