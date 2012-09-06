
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 09/06/2012 11:08:30
-- Generated from EDMX file: D:\Work\Txtfeedback\Repository Git\txtFeedback\SmsFeedback_EFModels\SmsFeedbackModels.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [txtfeedback_production];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CompanySubscriptions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Companies] DROP CONSTRAINT [FK_CompanySubscriptions];
GO
IF OBJECT_ID(N'[dbo].[FK_CompanyTag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tags] DROP CONSTRAINT [FK_CompanyTag];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conversations] DROP CONSTRAINT [FK_ConversationClient];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationConversationTags]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConversationTags] DROP CONSTRAINT [FK_ConversationConversationTags];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationMessage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_ConversationMessage];
GO
IF OBJECT_ID(N'[dbo].[FK_SupportConversationForWorkingPoint1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkingPoints] DROP CONSTRAINT [FK_SupportConversationForWorkingPoint1];
GO
IF OBJECT_ID(N'[dbo].[FK_TagConversationTags]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConversationTags] DROP CONSTRAINT [FK_TagConversationTags];
GO
IF OBJECT_ID(N'[dbo].[FK_TagTagTagType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TagTagTypes] DROP CONSTRAINT [FK_TagTagTagType];
GO
IF OBJECT_ID(N'[dbo].[FK_TagTypeTagTagType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TagTagTypes] DROP CONSTRAINT [FK_TagTypeTagTagType];
GO
IF OBJECT_ID(N'[dbo].[FK_UserCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMessages]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_UserMessages];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersForWorkingPoints_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UsersForWorkingPoints] DROP CONSTRAINT [FK_UsersForWorkingPoints_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersForWorkingPoints_WorkingPoint]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UsersForWorkingPoints] DROP CONSTRAINT [FK_UsersForWorkingPoints_WorkingPoint];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkingPointActivityLogWorkingPoint]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityLogWorkingPoints] DROP CONSTRAINT [FK_WorkingPointActivityLogWorkingPoint];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkingPointConversation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conversations] DROP CONSTRAINT [FK_WorkingPointConversation];
GO
IF OBJECT_ID(N'[dbo].[FK_MembershipApplication]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Memberships] DROP CONSTRAINT [FK_MembershipApplication];
GO
IF OBJECT_ID(N'[dbo].[FK_MembershipUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Memberships] DROP CONSTRAINT [FK_MembershipUser];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleApplication]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Roles] DROP CONSTRAINT [FK_RoleApplication];
GO
IF OBJECT_ID(N'[dbo].[FK_UserApplication]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserApplication];
GO
IF OBJECT_ID(N'[dbo].[FK_UserProfile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Profiles] DROP CONSTRAINT [FK_UserProfile];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersInRoleRole]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UsersInRoles] DROP CONSTRAINT [FK_UsersInRoleRole];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersInRoleUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UsersInRoles] DROP CONSTRAINT [FK_UsersInRoleUser];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ActivityLogWorkingPoints]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityLogWorkingPoints];
GO
IF OBJECT_ID(N'[dbo].[Applications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Applications];
GO
IF OBJECT_ID(N'[dbo].[Clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clients];
GO
IF OBJECT_ID(N'[dbo].[Companies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Companies];
GO
IF OBJECT_ID(N'[dbo].[Conversations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Conversations];
GO
IF OBJECT_ID(N'[dbo].[ConversationTags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ConversationTags];
GO
IF OBJECT_ID(N'[dbo].[Memberships]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Memberships];
GO
IF OBJECT_ID(N'[dbo].[Messages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Messages];
GO
IF OBJECT_ID(N'[dbo].[Profiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Profiles];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[Subscriptions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Subscriptions];
GO
IF OBJECT_ID(N'[dbo].[Tags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tags];
GO
IF OBJECT_ID(N'[dbo].[TagTagTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TagTagTypes];
GO
IF OBJECT_ID(N'[dbo].[TagTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TagTypes];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[UsersForWorkingPoints]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UsersForWorkingPoints];
GO
IF OBJECT_ID(N'[dbo].[UsersInRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UsersInRoles];
GO
IF OBJECT_ID(N'[dbo].[WorkingPoints]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkingPoints];
GO
IF OBJECT_ID(N'[dbo].[XmppConnections]', 'U') IS NOT NULL
    DROP TABLE [dbo].[XmppConnections];
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
    [SupportConversation] nvarchar(50)  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [ApplicationId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [UserName] nvarchar(50)  NOT NULL,
    [IsAnonymous] bit  NOT NULL,
    [LastActivityDate] datetime  NOT NULL,
    [XmppConnection_Id] int  NULL,
    [Company_Name] nvarchar(50)  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [ApplicationId] uniqueidentifier  NOT NULL,
    [RoleId] uniqueidentifier  NOT NULL,
    [RoleName] nvarchar(256)  NOT NULL,
    [Description] nvarchar(256)  NULL
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

-- Creating table 'Applications'
CREATE TABLE [dbo].[Applications] (
    [ApplicationName] nvarchar(235)  NOT NULL,
    [ApplicationId] uniqueidentifier  NOT NULL,
    [Description] nvarchar(256)  NULL
);
GO

-- Creating table 'Memberships'
CREATE TABLE [dbo].[Memberships] (
    [ApplicationId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [Password] nvarchar(128)  NOT NULL,
    [PasswordFormat] int  NOT NULL,
    [PasswordSalt] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [PasswordQuestion] nvarchar(256)  NULL,
    [PasswordAnswer] nvarchar(128)  NULL,
    [IsApproved] bit  NOT NULL,
    [IsLockedOut] bit  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [LastLoginDate] datetime  NOT NULL,
    [LastPasswordChangedDate] datetime  NOT NULL,
    [LastLockoutDate] datetime  NOT NULL,
    [FailedPasswordAttemptCount] int  NOT NULL,
    [FailedPasswordAttemptWindowStart] datetime  NOT NULL,
    [FailedPasswordAnswerAttemptCount] int  NOT NULL,
    [FailedPasswordAnswerAttemptWindowsStart] datetime  NOT NULL,
    [Comment] nvarchar(256)  NULL
);
GO

-- Creating table 'Profiles'
CREATE TABLE [dbo].[Profiles] (
    [UserId] uniqueidentifier  NOT NULL,
    [PropertyNames] nvarchar(4000)  NOT NULL,
    [PropertyValueStrings] nvarchar(4000)  NOT NULL,
    [PropertyValueBinary] varbinary(max)  NOT NULL,
    [LastUpdatedDate] datetime  NOT NULL
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

-- Creating table 'TagTypes'
CREATE TABLE [dbo].[TagTypes] (
    [Type] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'TagTagTypes'
CREATE TABLE [dbo].[TagTagTypes] (
    [IsDefault] bit  NOT NULL,
    [TagName] nvarchar(50)  NOT NULL,
    [TagCompanyName] nvarchar(50)  NOT NULL,
    [TagTypeType] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'EventTypes'
CREATE TABLE [dbo].[EventTypes] (
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UsersForWorkingPoints'
CREATE TABLE [dbo].[UsersForWorkingPoints] (
    [Users_UserId] uniqueidentifier  NOT NULL,
    [WorkingPoints_TelNumber] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'UsersInRoles'
CREATE TABLE [dbo].[UsersInRoles] (
    [Roles_RoleId] uniqueidentifier  NOT NULL,
    [Users_UserId] uniqueidentifier  NOT NULL
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

-- Creating primary key on [UserId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [RoleId] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([RoleId] ASC);
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

-- Creating primary key on [ApplicationId] in table 'Applications'
ALTER TABLE [dbo].[Applications]
ADD CONSTRAINT [PK_Applications]
    PRIMARY KEY CLUSTERED ([ApplicationId] ASC);
GO

-- Creating primary key on [UserId] in table 'Memberships'
ALTER TABLE [dbo].[Memberships]
ADD CONSTRAINT [PK_Memberships]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [UserId] in table 'Profiles'
ALTER TABLE [dbo].[Profiles]
ADD CONSTRAINT [PK_Profiles]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
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

-- Creating primary key on [Type] in table 'TagTypes'
ALTER TABLE [dbo].[TagTypes]
ADD CONSTRAINT [PK_TagTypes]
    PRIMARY KEY CLUSTERED ([Type] ASC);
GO

-- Creating primary key on [TagName], [TagCompanyName], [TagTypeType] in table 'TagTagTypes'
ALTER TABLE [dbo].[TagTagTypes]
ADD CONSTRAINT [PK_TagTagTypes]
    PRIMARY KEY CLUSTERED ([TagName], [TagCompanyName], [TagTypeType] ASC);
GO

-- Creating primary key on [Name] in table 'EventTypes'
ALTER TABLE [dbo].[EventTypes]
ADD CONSTRAINT [PK_EventTypes]
    PRIMARY KEY CLUSTERED ([Name] ASC);
GO

-- Creating primary key on [Users_UserId], [WorkingPoints_TelNumber] in table 'UsersForWorkingPoints'
ALTER TABLE [dbo].[UsersForWorkingPoints]
ADD CONSTRAINT [PK_UsersForWorkingPoints]
    PRIMARY KEY NONCLUSTERED ([Users_UserId], [WorkingPoints_TelNumber] ASC);
GO

-- Creating primary key on [Roles_RoleId], [Users_UserId] in table 'UsersInRoles'
ALTER TABLE [dbo].[UsersInRoles]
ADD CONSTRAINT [PK_UsersInRoles]
    PRIMARY KEY NONCLUSTERED ([Roles_RoleId], [Users_UserId] ASC);
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

-- Creating foreign key on [ApplicationId] in table 'Memberships'
ALTER TABLE [dbo].[Memberships]
ADD CONSTRAINT [FK_MembershipApplication]
    FOREIGN KEY ([ApplicationId])
    REFERENCES [dbo].[Applications]
        ([ApplicationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MembershipApplication'
CREATE INDEX [IX_FK_MembershipApplication]
ON [dbo].[Memberships]
    ([ApplicationId]);
GO

-- Creating foreign key on [ApplicationId] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [FK_RoleApplication]
    FOREIGN KEY ([ApplicationId])
    REFERENCES [dbo].[Applications]
        ([ApplicationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleApplication'
CREATE INDEX [IX_FK_RoleApplication]
ON [dbo].[Roles]
    ([ApplicationId]);
GO

-- Creating foreign key on [ApplicationId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserApplication]
    FOREIGN KEY ([ApplicationId])
    REFERENCES [dbo].[Applications]
        ([ApplicationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserApplication'
CREATE INDEX [IX_FK_UserApplication]
ON [dbo].[Users]
    ([ApplicationId]);
GO

-- Creating foreign key on [UserId] in table 'Memberships'
ALTER TABLE [dbo].[Memberships]
ADD CONSTRAINT [FK_MembershipUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [UserId] in table 'Profiles'
ALTER TABLE [dbo].[Profiles]
ADD CONSTRAINT [FK_UserProfile]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
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

-- Creating foreign key on [Roles_RoleId] in table 'UsersInRoles'
ALTER TABLE [dbo].[UsersInRoles]
ADD CONSTRAINT [FK_UsersInRoles_Role]
    FOREIGN KEY ([Roles_RoleId])
    REFERENCES [dbo].[Roles]
        ([RoleId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Users_UserId] in table 'UsersInRoles'
ALTER TABLE [dbo].[UsersInRoles]
ADD CONSTRAINT [FK_UsersInRoles_User]
    FOREIGN KEY ([Users_UserId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UsersInRoles_User'
CREATE INDEX [IX_FK_UsersInRoles_User]
ON [dbo].[UsersInRoles]
    ([Users_UserId]);
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

-- Creating foreign key on [SupportConversation] in table 'WorkingPoints'
ALTER TABLE [dbo].[WorkingPoints]
ADD CONSTRAINT [FK_SupportConversationForWorkingPoint1]
    FOREIGN KEY ([SupportConversation])
    REFERENCES [dbo].[Conversations]
        ([ConvId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SupportConversationForWorkingPoint1'
CREATE INDEX [IX_FK_SupportConversationForWorkingPoint1]
ON [dbo].[WorkingPoints]
    ([SupportConversation]);
GO

-- Creating foreign key on [TagName], [TagCompanyName] in table 'TagTagTypes'
ALTER TABLE [dbo].[TagTagTypes]
ADD CONSTRAINT [FK_TagTagTagType]
    FOREIGN KEY ([TagName], [TagCompanyName])
    REFERENCES [dbo].[Tags]
        ([Name], [CompanyName])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [TagTypeType] in table 'TagTagTypes'
ALTER TABLE [dbo].[TagTagTypes]
ADD CONSTRAINT [FK_TagTypeTagTagType]
    FOREIGN KEY ([TagTypeType])
    REFERENCES [dbo].[TagTypes]
        ([Type])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TagTypeTagTagType'
CREATE INDEX [IX_FK_TagTypeTagTagType]
ON [dbo].[TagTagTypes]
    ([TagTypeType]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------