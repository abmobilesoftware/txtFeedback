
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/30/2012 14:05:48
-- Generated from EDMX file: D:\Work\smsFeedback\SmsFeedback_EFModels\SmsFeedbackModels.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [txtfeedback_dev];
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
    [WorkingPoint_TelNumber] nvarchar(50)  NOT NULL,
    [Client_TelNumber] nvarchar(50)  NOT NULL
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
    [ConversationId] nvarchar(50)  NOT NULL,
    [ResponseTime] bigint  NULL,
    [UserUserId] uniqueidentifier  NULL
);
GO

-- Creating table 'WorkingPoints'
CREATE TABLE [dbo].[WorkingPoints] (
    [TelNumber] nvarchar(50)  NOT NULL,
    [Description] nvarchar(160)  NOT NULL,
    [Name] nvarchar(40)  NOT NULL,
    [Provider] nvarchar(50)  NOT NULL,
    [SentSms] int  NOT NULL,
    [MaxNrOfSmsToSend] int  NOT NULL,
    [Support_TelNumber] nvarchar(50)  NOT NULL,
    [SupportConversation_ConvId] nvarchar(50)  NOT NULL
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
    [Address] nvarchar(max)  NOT NULL,
    [NrOfTrainingHoursDelivered] int  NOT NULL,
    [Subscription_Type] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Clients'
CREATE TABLE [dbo].[Clients] (
    [TelNumber] nvarchar(50)  NOT NULL,
    [DisplayName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(160)  NOT NULL,
    [isSupportClient] bit  NOT NULL
);
GO

-- Creating table 'ConversationTags'
CREATE TABLE [dbo].[ConversationTags] (
    [DateAdded] datetime  NOT NULL,
    [ConversationConvId] nvarchar(50)  NOT NULL,
    [TagName] nvarchar(50)  NOT NULL,
    [TagCompanyName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'ActivityLogWorkingPoints'
CREATE TABLE [dbo].[ActivityLogWorkingPoints] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [SentSms] int  NOT NULL,
    [WorkingPoint_TelNumber] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Subscriptions'
CREATE TABLE [dbo].[Subscriptions] (
    [Type] nvarchar(50)  NOT NULL,
    [NrOfWorkingPoints] int  NOT NULL,
    [SmsPerWorkingPoint] int  NOT NULL,
    [NrOfUsers] int  NOT NULL,
    [NrOfHoursOfTraining] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UsersForWorkingPoints'
CREATE TABLE [dbo].[UsersForWorkingPoints] (
    [Users_UserId] uniqueidentifier  NOT NULL,
    [WorkingPoints_TelNumber] nvarchar(50)  NOT NULL
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

-- Creating primary key on [TelNumber] in table 'WorkingPoints'
ALTER TABLE [dbo].[WorkingPoints]
ADD CONSTRAINT [PK_WorkingPoints]
    PRIMARY KEY CLUSTERED ([TelNumber] ASC);
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

-- Creating primary key on [TelNumber] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [PK_Clients]
    PRIMARY KEY CLUSTERED ([TelNumber] ASC);
GO

-- Creating primary key on [ConversationConvId], [TagName], [TagCompanyName] in table 'ConversationTags'
ALTER TABLE [dbo].[ConversationTags]
ADD CONSTRAINT [PK_ConversationTags]
    PRIMARY KEY CLUSTERED ([ConversationConvId], [TagName], [TagCompanyName] ASC);
GO

-- Creating primary key on [Id] in table 'ActivityLogWorkingPoints'
ALTER TABLE [dbo].[ActivityLogWorkingPoints]
ADD CONSTRAINT [PK_ActivityLogWorkingPoints]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Type] in table 'Subscriptions'
ALTER TABLE [dbo].[Subscriptions]
ADD CONSTRAINT [PK_Subscriptions]
    PRIMARY KEY CLUSTERED ([Type] ASC);
GO

-- Creating primary key on [Users_UserId], [WorkingPoints_TelNumber] in table 'UsersForWorkingPoints'
ALTER TABLE [dbo].[UsersForWorkingPoints]
ADD CONSTRAINT [PK_UsersForWorkingPoints]
    PRIMARY KEY NONCLUSTERED ([Users_UserId], [WorkingPoints_TelNumber] ASC);
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

-- Creating foreign key on [WorkingPoints_TelNumber] in table 'UsersForWorkingPoints'
ALTER TABLE [dbo].[UsersForWorkingPoints]
ADD CONSTRAINT [FK_UsersForWorkingPoints_WorkingPoint]
    FOREIGN KEY ([WorkingPoints_TelNumber])
    REFERENCES [dbo].[WorkingPoints]
        ([TelNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UsersForWorkingPoints_WorkingPoint'
CREATE INDEX [IX_FK_UsersForWorkingPoints_WorkingPoint]
ON [dbo].[UsersForWorkingPoints]
    ([WorkingPoints_TelNumber]);
GO

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

-- Creating foreign key on [WorkingPoint_TelNumber] in table 'Conversations'
ALTER TABLE [dbo].[Conversations]
ADD CONSTRAINT [FK_WorkingPointConversation]
    FOREIGN KEY ([WorkingPoint_TelNumber])
    REFERENCES [dbo].[WorkingPoints]
        ([TelNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkingPointConversation'
CREATE INDEX [IX_FK_WorkingPointConversation]
ON [dbo].[Conversations]
    ([WorkingPoint_TelNumber]);
GO

-- Creating foreign key on [UserUserId] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_UserMessages]
    FOREIGN KEY ([UserUserId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserMessages'
CREATE INDEX [IX_FK_UserMessages]
ON [dbo].[Messages]
    ([UserUserId]);
GO

-- Creating foreign key on [Client_TelNumber] in table 'Conversations'
ALTER TABLE [dbo].[Conversations]
ADD CONSTRAINT [FK_ConversationClient]
    FOREIGN KEY ([Client_TelNumber])
    REFERENCES [dbo].[Clients]
        ([TelNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConversationClient'
CREATE INDEX [IX_FK_ConversationClient]
ON [dbo].[Conversations]
    ([Client_TelNumber]);
GO

-- Creating foreign key on [Support_TelNumber] in table 'WorkingPoints'
ALTER TABLE [dbo].[WorkingPoints]
ADD CONSTRAINT [FK_WorkingPointClient]
    FOREIGN KEY ([Support_TelNumber])
    REFERENCES [dbo].[Clients]
        ([TelNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkingPointClient'
CREATE INDEX [IX_FK_WorkingPointClient]
ON [dbo].[WorkingPoints]
    ([Support_TelNumber]);
GO

-- Creating foreign key on [SupportConversation_ConvId] in table 'WorkingPoints'
ALTER TABLE [dbo].[WorkingPoints]
ADD CONSTRAINT [FK_SupportWorkingPointConversation]
    FOREIGN KEY ([SupportConversation_ConvId])
    REFERENCES [dbo].[Conversations]
        ([ConvId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SupportWorkingPointConversation'
CREATE INDEX [IX_FK_SupportWorkingPointConversation]
ON [dbo].[WorkingPoints]
    ([SupportConversation_ConvId]);
GO

-- Creating foreign key on [ConversationConvId] in table 'ConversationTags'
ALTER TABLE [dbo].[ConversationTags]
ADD CONSTRAINT [FK_ConversationConversationTags]
    FOREIGN KEY ([ConversationConvId])
    REFERENCES [dbo].[Conversations]
        ([ConvId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [TagName], [TagCompanyName] in table 'ConversationTags'
ALTER TABLE [dbo].[ConversationTags]
ADD CONSTRAINT [FK_TagConversationTags]
    FOREIGN KEY ([TagName], [TagCompanyName])
    REFERENCES [dbo].[Tags]
        ([Name], [CompanyName])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TagConversationTags'
CREATE INDEX [IX_FK_TagConversationTags]
ON [dbo].[ConversationTags]
    ([TagName], [TagCompanyName]);
GO


-- Creating foreign key on [WorkingPoint_TelNumber] in table 'ActivityLogWorkingPoints'
ALTER TABLE [dbo].[ActivityLogWorkingPoints]
ADD CONSTRAINT [FK_WorkingPointActivityLogWorkingPoint]
    FOREIGN KEY ([WorkingPoint_TelNumber])
    REFERENCES [dbo].[WorkingPoints]
        ([TelNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkingPointActivityLogWorkingPoint'
CREATE INDEX [IX_FK_WorkingPointActivityLogWorkingPoint]
ON [dbo].[ActivityLogWorkingPoints]
    ([WorkingPoint_TelNumber]);
GO

-- Creating foreign key on [Subscription_Type] in table 'Companies'
ALTER TABLE [dbo].[Companies]
ADD CONSTRAINT [FK_CompanySubscriptions]
    FOREIGN KEY ([Subscription_Type])
    REFERENCES [dbo].[Subscriptions]
        ([Type])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CompanySubscriptions'
CREATE INDEX [IX_FK_CompanySubscriptions]
ON [dbo].[Companies]
    ([Subscription_Type]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------