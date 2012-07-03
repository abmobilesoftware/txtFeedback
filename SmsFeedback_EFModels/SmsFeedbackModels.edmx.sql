
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/03/2012 14:17:53
-- Generated from EDMX file: D:\Work\smsFeedback\SmsFeedback_EFModels\SmsFeedbackModels.edmx
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

IF OBJECT_ID(N'[dbo].[FK_CompanyTag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tags] DROP CONSTRAINT [FK_CompanyTag];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationMessage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_ConversationMessage];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationTags_Conversation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConversationTags] DROP CONSTRAINT [FK_ConversationTags_Conversation];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationTags_Tag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConversationTags] DROP CONSTRAINT [FK_ConversationTags_Tag];
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
IF OBJECT_ID(N'[dbo].[ConversationTags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ConversationTags];
GO
IF OBJECT_ID(N'[dbo].[Messages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Messages];
GO

IF OBJECT_ID(N'[dbo].[Tags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tags];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Conversations'
CREATE TABLE [dbo].[Conversations] (
    [ConvId] nvarchar(50)  NOT NULL,
    [Text] nvarchar(160)  NOT NULL,
    [Read] bit  NOT NULL,
    [TimeUpdated] datetime  NOT NULL,
    [WorkingPointId] int  NOT NULL,
    [From] nvarchar(max)  NOT NULL,
    [Starred] bit  NOT NULL,
    [WorkingPoint_Id] int  NOT NULL
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
    [ConversationId] int  NOT NULL,
    [ConversationConvId] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Name] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [CompanyName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'ConversationTags'
CREATE TABLE [dbo].[ConversationTags] (
    [Conversations_ConvId] nvarchar(50)  NOT NULL,
    [Tags_Name] nvarchar(50)  NOT NULL,
    [Tags_CompanyName] nvarchar(50)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ConvId] in table 'Conversations'
ALTER TABLE [dbo].[Conversations]
ADD CONSTRAINT [PK_Conversations]
    PRIMARY KEY CLUSTERED ([ConvId] ASC);
GO

-- Creating primary key on [Id] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [PK_Messages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Name], [CompanyName] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([Name], [CompanyName] ASC);
GO


-- Creating primary key on [Conversations_ConvId], [Tags_Name], [Tags_CompanyName] in table 'ConversationTags'
ALTER TABLE [dbo].[ConversationTags]
ADD CONSTRAINT [PK_ConversationTags]
    PRIMARY KEY NONCLUSTERED ([Conversations_ConvId], [Tags_Name], [Tags_CompanyName] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------


-- Creating foreign key on [CompanyName] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [FK_CompanyTag]
    FOREIGN KEY ([CompanyName])
    REFERENCES [dbo].[Companies]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CompanyTag'
CREATE INDEX [IX_FK_CompanyTag]
ON [dbo].[Tags]
    ([CompanyName]);
GO

-- Creating foreign key on [Conversations_ConvId] in table 'ConversationTags'
ALTER TABLE [dbo].[ConversationTags]
ADD CONSTRAINT [FK_ConversationTags_Conversation]
    FOREIGN KEY ([Conversations_ConvId])
    REFERENCES [dbo].[Conversations]
        ([ConvId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Tags_Name], [Tags_CompanyName] in table 'ConversationTags'
ALTER TABLE [dbo].[ConversationTags]
ADD CONSTRAINT [FK_ConversationTags_Tag]
    FOREIGN KEY ([Tags_Name], [Tags_CompanyName])
    REFERENCES [dbo].[Tags]
        ([Name], [CompanyName])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConversationTags_Tag'
CREATE INDEX [IX_FK_ConversationTags_Tag]
ON [dbo].[ConversationTags]
    ([Tags_Name], [Tags_CompanyName]);
GO

-- Creating foreign key on [WorkingPoint_Id] in table 'Conversations'
ALTER TABLE [dbo].[Conversations]
ADD CONSTRAINT [FK_WorkingPointConversation]
    FOREIGN KEY ([WorkingPoint_Id])
    REFERENCES [dbo].[WorkingPoints]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkingPointConversation'
CREATE INDEX [IX_FK_WorkingPointConversation]
ON [dbo].[Conversations]
    ([WorkingPoint_Id]);
GO

-- Creating foreign key on [ConversationConvId] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_ConversationMessage]
    FOREIGN KEY ([ConversationConvId])
    REFERENCES [dbo].[Conversations]
        ([ConvId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConversationMessage'
CREATE INDEX [IX_FK_ConversationMessage]
ON [dbo].[Messages]
    ([ConversationConvId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------