CREATE TABLE [dbo].[Eskom_Schedules] (
    [Id]         INT      IDENTITY (1, 1) NOT NULL,
    [SuburbID]   INT      NULL,
    [Stage]      INT      NULL,
    [DayOfMonth] INT      NULL,
    [StartTime]  TIME (7) NULL,
    [EndTime]    TIME (7) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

