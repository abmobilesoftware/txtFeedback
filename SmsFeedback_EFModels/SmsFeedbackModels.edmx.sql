
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/26/2012 11:59:15
-- Generated from EDMX file: D:\Work\smsFeedback\SmsFeedback_EFModels\SmsFeedbackModels.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [txtfeedback_nexmo];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
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
    [From] nvarchar(max)  NOT NULL,
    [Starred] bit  NOT NULL,
    [StartTime] datetime  NOT NULL,
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
    [ConversationId] nvarchar(50)  NOT NULL
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

-- Creating table 'XmppConnections'
CREATE TABLE [dbo].[XmppConnections] (
    [XmppUser] nvarchar(max)  NOT NULL,
    [XmppPassword] nvarchar(max)  NOT NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Name] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [CompanyName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Companies'
CREATE TABLE [dbo].[Companies] (
    [Name] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UsersForWorkingPoints'
CREATE TABLE [dbo].[UsersForWorkingPoints] (
    [Users_UserId] uniqueidentifier  NOT NULL,
    [WorkingPoints_Id] int  NOT NULL
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

-- Creating primary key on [Id] in table 'WorkingPoints'
ALTER TABLE [dbo].[WorkingPoints]
ADD CONSTRAINT [PK_WorkingPoints]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO


-- Creating primary key on [Id] in table 'XmppConnections'
ALTER TABLE [dbo].[XmppConnections]
ADD CONSTRAINT [PK_XmppConnections]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Name], [CompanyName] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([Name], [CompanyName] ASC);
GO

-- Creating primary key on [Name] in table 'Companies'
ALTER TABLE [dbo].[Companies]
ADD CONSTRAINT [PK_Companies]
    PRIMARY KEY CLUSTERED ([Name] ASC);
GO

-- Creating primary key on [Users_UserId], [WorkingPoints_Id] in table 'UsersForWorkingPoints'
ALTER TABLE [dbo].[UsersForWorkingPoints]
ADD CONSTRAINT [PK_UsersForWorkingPoints]
    PRIMARY KEY NONCLUSTERED ([Users_UserId], [WorkingPoints_Id] ASC);
GO

-- Creating primary key on [Conversations_ConvId], [Tags_Name], [Tags_CompanyName] in table 'ConversationTags'
ALTER TABLE [dbo].[ConversationTags]
ADD CONSTRAINT [PK_ConversationTags]
    PRIMARY KEY NONCLUSTERED ([Conversations_ConvId], [Tags_Name], [Tags_CompanyName] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Users_UserId] in table 'UsersForWorkingPoints'
ALTER TABLE [dbo].[UsersForWorkingPoints]
ADD CONSTRAINT [FK_UsersForWorkingPoints_User]
    FOREIGN KEY ([Users_UserId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [WorkingPoints_Id] in table 'UsersForWorkingPoints'
ALTER TABLE [dbo].[UsersForWorkingPoints]
ADD CONSTRAINT [FK_UsersForWorkingPoints_WorkingPoint]
    FOREIGN KEY ([WorkingPoints_Id])
    REFERENCES [dbo].[WorkingPoints]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UsersForWorkingPoints_WorkingPoint'
CREATE INDEX [IX_FK_UsersForWorkingPoints_WorkingPoint]
ON [dbo].[UsersForWorkingPoints]
    ([WorkingPoints_Id]);
GO

-- Creating foreign key on [XmppConnection_Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserXmppConnection]
    FOREIGN KEY ([XmppConnection_Id])
    REFERENCES [dbo].[XmppConnections]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserXmppConnection'
CREATE INDEX [IX_FK_UserXmppConnection]
ON [dbo].[Users]
    ([XmppConnection_Id]);
GO

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

-- Creating foreign key on [Company_Name] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserCompany]
    FOREIGN KEY ([Company_Name])
    REFERENCES [dbo].[Companies]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserCompany'
CREATE INDEX [IX_FK_UserCompany]
ON [dbo].[Users]
    ([Company_Name]);
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

-- Creating foreign key on [ConversationId] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_ConversationMessage]
    FOREIGN KEY ([ConversationId])
    REFERENCES [dbo].[Conversations]
        ([ConvId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConversationMessage'
CREATE INDEX [IX_FK_ConversationMessage]
ON [dbo].[Messages]
    ([ConversationId]);
GO


-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------