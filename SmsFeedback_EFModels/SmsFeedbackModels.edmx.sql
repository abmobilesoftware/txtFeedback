
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 05/25/2012 17:25:35
-- Generated from EDMX file: C:\Users\Ando\documents\visual studio 11\Projects\SmsFeedback_Take4\SmsFeedback_EFModels\SmsFeedbackModels.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [sms_feedback_2];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ConversationMessage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_ConversationMessage];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkingPointConversation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conversations] DROP CONSTRAINT [FK_WorkingPointConversation];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Conversations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Conversations];
GO
IF OBJECT_ID(N'[dbo].[Messages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Messages];
GO
IF OBJECT_ID(N'[dbo].[WorkingPoints]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkingPoints];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Conversations'
CREATE TABLE [dbo].[Conversations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ConvId] nvarchar(max)  NOT NULL,
    [Text] nvarchar(160)  NOT NULL,
    [Read] bit  NOT NULL,
    [TimeUpdated] datetime  NOT NULL,
    [WorkingPointId] int  NOT NULL,
    [From] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Messages'
CREATE TABLE [dbo].[Messages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [From] nvarchar(30)  NOT NULL,
    [To] nvarchar(30)  NOT NULL,
    [Text] nvarchar(160)  NOT NULL,
    [TimeReceived] datetime  NOT NULL,
    [Read] bit  NOT NULL,
    [ConversationId] int  NOT NULL
);
GO

-- Creating table 'WorkingPoints'
CREATE TABLE [dbo].[WorkingPoints] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TelNumber] nvarchar(max)  NOT NULL,
    [Description] nvarchar(160)  NOT NULL,
    [Name] nvarchar(40)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Conversations'
ALTER TABLE [dbo].[Conversations]
ADD CONSTRAINT [PK_Conversations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [PK_Messages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkingPoints'
ALTER TABLE [dbo].[WorkingPoints]
ADD CONSTRAINT [PK_WorkingPoints]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ConversationId] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_ConversationMessage]
    FOREIGN KEY ([ConversationId])
    REFERENCES [dbo].[Conversations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConversationMessage'
CREATE INDEX [IX_FK_ConversationMessage]
ON [dbo].[Messages]
    ([ConversationId]);
GO

-- Creating foreign key on [WorkingPointId] in table 'Conversations'
ALTER TABLE [dbo].[Conversations]
ADD CONSTRAINT [FK_WorkingPointConversation]
    FOREIGN KEY ([WorkingPointId])
    REFERENCES [dbo].[WorkingPoints]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkingPointConversation'
CREATE INDEX [IX_FK_WorkingPointConversation]
ON [dbo].[Conversations]
    ([WorkingPointId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------