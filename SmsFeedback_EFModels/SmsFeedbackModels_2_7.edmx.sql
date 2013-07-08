
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/02/2013 16:38:42
-- Generated from EDMX file: D:\Work\Txtfeedback\Repository Git\txtFeedback\SmsFeedback_EFModels\SmsFeedbackModels.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [txtfeedback_dev];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UsersForWorkingPoints_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UsersForWorkingPoints] DROP CONSTRAINT [FK_UsersForWorkingPoints_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersForWorkingPoints_WorkingPoint]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UsersForWorkingPoints] DROP CONSTRAINT [FK_UsersForWorkingPoints_WorkingPoint];
GO
IF OBJECT_ID(N'[dbo].[FK_CompanyTag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tags] DROP CONSTRAINT [FK_CompanyTag];
GO
IF OBJECT_ID(N'[dbo].[FK_UserCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationMessage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_ConversationMessage];
GO
IF OBJECT_ID(N'[dbo].[FK_MembershipApplication]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Memberships] DROP CONSTRAINT [FK_MembershipApplication];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleApplication]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Roles] DROP CONSTRAINT [FK_RoleApplication];
GO
IF OBJECT_ID(N'[dbo].[FK_UserApplication]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserApplication];
GO
IF OBJECT_ID(N'[dbo].[FK_MembershipUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Memberships] DROP CONSTRAINT [FK_MembershipUser];
GO
IF OBJECT_ID(N'[dbo].[FK_UserProfile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Profiles] DROP CONSTRAINT [FK_UserProfile];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkingPointConversation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conversations] DROP CONSTRAINT [FK_WorkingPointConversation];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conversations] DROP CONSTRAINT [FK_ConversationClient];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationConversationTags]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConversationTags] DROP CONSTRAINT [FK_ConversationConversationTags];
GO
IF OBJECT_ID(N'[dbo].[FK_TagConversationTags]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConversationTags] DROP CONSTRAINT [FK_TagConversationTags];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersInRoles_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UsersInRoles] DROP CONSTRAINT [FK_UsersInRoles_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersInRoles_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UsersInRoles] DROP CONSTRAINT [FK_UsersInRoles_User];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkingPointActivityLogWorkingPoint]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityLogWorkingPoints] DROP CONSTRAINT [FK_WorkingPointActivityLogWorkingPoint];
GO
IF OBJECT_ID(N'[dbo].[FK_SupportConversationForWorkingPoint1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkingPoints] DROP CONSTRAINT [FK_SupportConversationForWorkingPoint1];
GO
IF OBJECT_ID(N'[dbo].[FK_TagTagTagType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TagTagTypes] DROP CONSTRAINT [FK_TagTagTagType];
GO
IF OBJECT_ID(N'[dbo].[FK_TagTypeTagTagType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TagTagTypes] DROP CONSTRAINT [FK_TagTypeTagTagType];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationHistoryEventType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConversationHistories] DROP CONSTRAINT [FK_ConversationHistoryEventType];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationToConversationHistory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConversationHistories] DROP CONSTRAINT [FK_ConversationToConversationHistory];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationHistoryMessage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConversationHistories] DROP CONSTRAINT [FK_ConversationHistoryMessage];
GO
IF OBJECT_ID(N'[dbo].[FK_UserXmppConnection]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserXmppConnection];
GO
IF OBJECT_ID(N'[dbo].[FK_MessageXmppConnection]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_MessageXmppConnection];
GO
IF OBJECT_ID(N'[dbo].[FK_CompanyInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_CompanyInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_InvoiceInvoiceDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[InvoiceDetails] DROP CONSTRAINT [FK_InvoiceInvoiceDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_SubscriptionDetailPrimaryContact]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubscriptionDetails] DROP CONSTRAINT [FK_SubscriptionDetailPrimaryContact];
GO
IF OBJECT_ID(N'[dbo].[FK_SubscriptionDetailSecondaryContact]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubscriptionDetails] DROP CONSTRAINT [FK_SubscriptionDetailSecondaryContact];
GO
IF OBJECT_ID(N'[dbo].[FK_CompanyContact]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Companies] DROP CONSTRAINT [FK_CompanyContact];
GO
IF OBJECT_ID(N'[dbo].[FK_CompanySubscriptionDetails1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Companies] DROP CONSTRAINT [FK_CompanySubscriptionDetails1];
GO
IF OBJECT_ID(N'[dbo].[FK_SubscriptionDetailInvoiceDetailsTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubscriptionDetails] DROP CONSTRAINT [FK_SubscriptionDetailInvoiceDetailsTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_SubscriptionDetailInvoiceDetailsTemplate1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubscriptionDetails] DROP CONSTRAINT [FK_SubscriptionDetailInvoiceDetailsTemplate1];
GO
IF OBJECT_ID(N'[dbo].[FK_UserDevice_Device]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserDevice] DROP CONSTRAINT [FK_UserDevice_Device];
GO
IF OBJECT_ID(N'[dbo].[FK_UserDevice_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserDevice] DROP CONSTRAINT [FK_UserDevice_User];
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
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[XmppConnections]', 'U') IS NOT NULL
    DROP TABLE [dbo].[XmppConnections];
GO
IF OBJECT_ID(N'[dbo].[Tags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tags];
GO
IF OBJECT_ID(N'[dbo].[Companies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Companies];
GO
IF OBJECT_ID(N'[dbo].[Applications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Applications];
GO
IF OBJECT_ID(N'[dbo].[Memberships]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Memberships];
GO
IF OBJECT_ID(N'[dbo].[Profiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Profiles];
GO
IF OBJECT_ID(N'[dbo].[Clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clients];
GO
IF OBJECT_ID(N'[dbo].[ConversationTags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ConversationTags];
GO
IF OBJECT_ID(N'[dbo].[ActivityLogWorkingPoints]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityLogWorkingPoints];
GO
IF OBJECT_ID(N'[dbo].[TagTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TagTypes];
GO
IF OBJECT_ID(N'[dbo].[TagTagTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TagTagTypes];
GO
IF OBJECT_ID(N'[dbo].[EventTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EventTypes];
GO
IF OBJECT_ID(N'[dbo].[ConversationHistories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ConversationHistories];
GO
IF OBJECT_ID(N'[dbo].[InvoiceDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InvoiceDetails];
GO
IF OBJECT_ID(N'[dbo].[Invoices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Invoices];
GO
IF OBJECT_ID(N'[dbo].[SubscriptionDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubscriptionDetails];
GO
IF OBJECT_ID(N'[dbo].[Contacts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Contacts];
GO
IF OBJECT_ID(N'[dbo].[InvoiceDetailsTemplates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InvoiceDetailsTemplates];
GO
IF OBJECT_ID(N'[dbo].[Logs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Logs];
GO
IF OBJECT_ID(N'[dbo].[Devices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices];
GO
IF OBJECT_ID(N'[dbo].[UsersForWorkingPoints]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UsersForWorkingPoints];
GO
IF OBJECT_ID(N'[dbo].[UsersInRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UsersInRoles];
GO
IF OBJECT_ID(N'[dbo].[UserDevice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserDevice];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Conversations'
CREATE TABLE [dbo].[Conversations] (
    [ConvId] nvarchar(50)  NOT NULL,
    [Text] nvarchar(500)  NULL,
    [Read] bit  NOT NULL,
    [TimeUpdated] datetime  NOT NULL,
    [From] nvarchar(100)  NULL,
    [Starred] bit  NOT NULL,
    [StartTime] datetime  NOT NULL,
    [WorkingPoint_TelNumber] nvarchar(50)  NOT NULL,
    [LastSequence] int  NOT NULL,
    [IsSmsBased] bit  NOT NULL,
    [Client_TelNumber] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Messages'
CREATE TABLE [dbo].[Messages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [From] nvarchar(100)  NULL,
    [To] nvarchar(100)  NULL,
    [Text] nvarchar(500)  NULL,
    [TimeReceived] datetime  NOT NULL,
    [Read] bit  NOT NULL,
    [ConversationId] nvarchar(50)  NOT NULL,
    [ResponseTime] bigint  NULL,
    [IsSmsBased] bit  NOT NULL,
    [XmppConnectionXmppUser] nvarchar(100)  NULL,
    [ExternalID] nvarchar(60)  NULL,
    [Price] nvarchar(100)  NULL,
    [ClientAcknowledge] bit  NOT NULL
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
    [SupportConversation] nvarchar(50)  NULL,
    [ShortID] nvarchar(10)  NOT NULL,
    [WelcomeMessage] nvarchar(160)  NULL,
    [XMPPsuffix] nvarchar(50)  NOT NULL,
    [BusyMessage] nvarchar(160)  NULL,
    [OutsideOfficeHoursMessage] nvarchar(160)  NULL,
    [Language] nvarchar(10)  NOT NULL,
    [BusyMessageTimer] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [ApplicationId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [UserName] nvarchar(50)  NOT NULL,
    [IsAnonymous] bit  NOT NULL,
    [LastActivityDate] datetime  NOT NULL,
    [Company_Name] nvarchar(50)  NULL,
    [XmppConnectionXmppUser] nvarchar(100)  NOT NULL,
    [ActivityReportDelivery] nvarchar(255)  NOT NULL,
    [SoundNotifications] bit  NOT NULL
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
    [XmppUser] nvarchar(100)  NOT NULL,
    [XmppPassword] nvarchar(max)  NOT NULL
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
    [Description] nvarchar(500)  NOT NULL,
    [Address] nvarchar(500)  NOT NULL,
    [RegistrationNumber] nvarchar(50)  NULL,
    [PostalCode] nvarchar(50)  NULL,
    [City] nvarchar(70)  NOT NULL,
    [Notes] nvarchar(500)  NULL,
    [VATID] nvarchar(50)  NOT NULL,
    [Contact_Id] int  NOT NULL,
    [Bank] nvarchar(50)  NULL,
    [BankAccount] nvarchar(70)  NULL,
    [SubscriptionDetails_Id] int  NOT NULL
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
    [Name] nvarchar(50)  NOT NULL,
    [Description] nvarchar(200)  NOT NULL,
    [FriendlyName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'ConversationHistories'
CREATE TABLE [dbo].[ConversationHistories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Sequence] int  NOT NULL,
    [Date] datetime  NOT NULL,
    [EventTypeName] nvarchar(50)  NULL,
    [ConversationConvId] nvarchar(50)  NOT NULL,
    [MessageId] int  NOT NULL
);
GO

-- Creating table 'InvoiceDetails'
CREATE TABLE [dbo].[InvoiceDetails] (
    [InvoiceDetailsID] int IDENTITY(1,1) NOT NULL,
    [Article] nvarchar(200)  NOT NULL,
    [Quantity] int  NOT NULL,
    [Price] decimal(15,5)  NOT NULL,
    [DateCreated] datetime  NOT NULL,
    [InvoiceInvoiceId] int  NOT NULL,
    [VAT] decimal(15,5)  NOT NULL
);
GO

-- Creating table 'Invoices'
CREATE TABLE [dbo].[Invoices] (
    [InvoiceId] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NULL,
    [Notes] nvarchar(500)  NOT NULL,
    [ProposalDetails] nvarchar(500)  NULL,
    [DateCreated] datetime  NOT NULL,
    [DueDate] datetime  NOT NULL,
    [AdvancePaymentTax] decimal(15,5)  NULL,
    [Paid] bit  NOT NULL,
    [CompanyName] nvarchar(50)  NOT NULL,
    [InvoiceNumber] int  NOT NULL,
    [Currency] nvarchar(15)  NOT NULL,
    [AutoGenerated] bit  NOT NULL
);
GO

-- Creating table 'SubscriptionDetails'
CREATE TABLE [dbo].[SubscriptionDetails] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [BillingDay] int  NOT NULL,
    [SubscriptionSMS] int  NOT NULL,
    [RemainingSMS] int  NOT NULL,
    [SpendingLimit] decimal(15,5)  NOT NULL,
    [WarningLimit] decimal(15,5)  NOT NULL,
    [SpentThisMonth] decimal(15,5)  NOT NULL,
    [PrimaryContact_Id] int  NOT NULL,
    [SecondaryContact_Id] int  NOT NULL,
    [DefaultCurrency] nvarchar(15)  NOT NULL,
    [MonthlySubscriptionTemplate_InvoiceDetailsTemplateID] int  NULL,
    [MonthlyExtraSMSCharge_InvoiceDetailsTemplateID] int  NULL,
    [ExtraAddedCreditThisMonth] decimal(15,5)  NOT NULL,
    [RemainingCreditFromPreviousMonth] decimal(15,5)  NOT NULL,
    [TimeStamp] binary(8)  NOT NULL
);
GO

-- Creating table 'Contacts'
CREATE TABLE [dbo].[Contacts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Surname] nvarchar(70)  NOT NULL,
    [Email] nvarchar(50)  NULL,
    [Mobile] nvarchar(50)  NULL,
    [Landline] nvarchar(50)  NULL
);
GO

-- Creating table 'InvoiceDetailsTemplates'
CREATE TABLE [dbo].[InvoiceDetailsTemplates] (
    [InvoiceDetailsTemplateID] int IDENTITY(1,1) NOT NULL,
    [Article] nvarchar(200)  NOT NULL,
    [Quantity] int  NOT NULL,
    [Price] decimal(15,5)  NOT NULL,
    [VAT] decimal(15,5)  NOT NULL
);
GO

-- Creating table 'Logs'
CREATE TABLE [dbo].[Logs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [Thread] varchar(255)  NOT NULL,
    [Level] varchar(50)  NOT NULL,
    [Logger] varchar(255)  NOT NULL,
    [Message] varchar(4000)  NOT NULL,
    [Exception] varchar(2000)  NULL
);
GO

-- Creating table 'Devices'
CREATE TABLE [dbo].[Devices] (
    [Id] nvarchar(300)  NOT NULL
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

-- Creating table 'UserDevice'
CREATE TABLE [dbo].[UserDevice] (
    [Devices_Id] nvarchar(300)  NOT NULL,
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

-- Creating primary key on [XmppUser] in table 'XmppConnections'
ALTER TABLE [dbo].[XmppConnections]
ADD CONSTRAINT [PK_XmppConnections]
    PRIMARY KEY CLUSTERED ([XmppUser] ASC);
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

-- Creating primary key on [Id] in table 'ConversationHistories'
ALTER TABLE [dbo].[ConversationHistories]
ADD CONSTRAINT [PK_ConversationHistories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [InvoiceDetailsID] in table 'InvoiceDetails'
ALTER TABLE [dbo].[InvoiceDetails]
ADD CONSTRAINT [PK_InvoiceDetails]
    PRIMARY KEY CLUSTERED ([InvoiceDetailsID] ASC);
GO

-- Creating primary key on [InvoiceId] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [PK_Invoices]
    PRIMARY KEY CLUSTERED ([InvoiceId] ASC);
GO

-- Creating primary key on [Id] in table 'SubscriptionDetails'
ALTER TABLE [dbo].[SubscriptionDetails]
ADD CONSTRAINT [PK_SubscriptionDetails]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Contacts'
ALTER TABLE [dbo].[Contacts]
ADD CONSTRAINT [PK_Contacts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [InvoiceDetailsTemplateID] in table 'InvoiceDetailsTemplates'
ALTER TABLE [dbo].[InvoiceDetailsTemplates]
ADD CONSTRAINT [PK_InvoiceDetailsTemplates]
    PRIMARY KEY CLUSTERED ([InvoiceDetailsTemplateID] ASC);
GO

-- Creating primary key on [Id], [Date], [Thread], [Level], [Logger], [Message] in table 'Logs'
ALTER TABLE [dbo].[Logs]
ADD CONSTRAINT [PK_Logs]
    PRIMARY KEY CLUSTERED ([Id], [Date], [Thread], [Level], [Logger], [Message] ASC);
GO

-- Creating primary key on [Id] in table 'Devices'
ALTER TABLE [dbo].[Devices]
ADD CONSTRAINT [PK_Devices]
    PRIMARY KEY CLUSTERED ([Id] ASC);
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

-- Creating primary key on [Devices_Id], [Users_UserId] in table 'UserDevice'
ALTER TABLE [dbo].[UserDevice]
ADD CONSTRAINT [PK_UserDevice]
    PRIMARY KEY NONCLUSTERED ([Devices_Id], [Users_UserId] ASC);
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

-- Creating foreign key on [EventTypeName] in table 'ConversationHistories'
ALTER TABLE [dbo].[ConversationHistories]
ADD CONSTRAINT [FK_ConversationHistoryEventType]
    FOREIGN KEY ([EventTypeName])
    REFERENCES [dbo].[EventTypes]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConversationHistoryEventType'
CREATE INDEX [IX_FK_ConversationHistoryEventType]
ON [dbo].[ConversationHistories]
    ([EventTypeName]);
GO

-- Creating foreign key on [ConversationConvId] in table 'ConversationHistories'
ALTER TABLE [dbo].[ConversationHistories]
ADD CONSTRAINT [FK_ConversationToConversationHistory]
    FOREIGN KEY ([ConversationConvId])
    REFERENCES [dbo].[Conversations]
        ([ConvId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConversationToConversationHistory'
CREATE INDEX [IX_FK_ConversationToConversationHistory]
ON [dbo].[ConversationHistories]
    ([ConversationConvId]);
GO

-- Creating foreign key on [MessageId] in table 'ConversationHistories'
ALTER TABLE [dbo].[ConversationHistories]
ADD CONSTRAINT [FK_ConversationHistoryMessage]
    FOREIGN KEY ([MessageId])
    REFERENCES [dbo].[Messages]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConversationHistoryMessage'
CREATE INDEX [IX_FK_ConversationHistoryMessage]
ON [dbo].[ConversationHistories]
    ([MessageId]);
GO

-- Creating foreign key on [XmppConnectionXmppUser] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserXmppConnection]
    FOREIGN KEY ([XmppConnectionXmppUser])
    REFERENCES [dbo].[XmppConnections]
        ([XmppUser])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserXmppConnection'
CREATE INDEX [IX_FK_UserXmppConnection]
ON [dbo].[Users]
    ([XmppConnectionXmppUser]);
GO

-- Creating foreign key on [XmppConnectionXmppUser] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_MessageXmppConnection]
    FOREIGN KEY ([XmppConnectionXmppUser])
    REFERENCES [dbo].[XmppConnections]
        ([XmppUser])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MessageXmppConnection'
CREATE INDEX [IX_FK_MessageXmppConnection]
ON [dbo].[Messages]
    ([XmppConnectionXmppUser]);
GO

-- Creating foreign key on [CompanyName] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [FK_CompanyInvoice]
    FOREIGN KEY ([CompanyName])
    REFERENCES [dbo].[Companies]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CompanyInvoice'
CREATE INDEX [IX_FK_CompanyInvoice]
ON [dbo].[Invoices]
    ([CompanyName]);
GO

-- Creating foreign key on [InvoiceInvoiceId] in table 'InvoiceDetails'
ALTER TABLE [dbo].[InvoiceDetails]
ADD CONSTRAINT [FK_InvoiceInvoiceDetails]
    FOREIGN KEY ([InvoiceInvoiceId])
    REFERENCES [dbo].[Invoices]
        ([InvoiceId])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_InvoiceInvoiceDetails'
CREATE INDEX [IX_FK_InvoiceInvoiceDetails]
ON [dbo].[InvoiceDetails]
    ([InvoiceInvoiceId]);
GO

-- Creating foreign key on [PrimaryContact_Id] in table 'SubscriptionDetails'
ALTER TABLE [dbo].[SubscriptionDetails]
ADD CONSTRAINT [FK_SubscriptionDetailPrimaryContact]
    FOREIGN KEY ([PrimaryContact_Id])
    REFERENCES [dbo].[Contacts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SubscriptionDetailPrimaryContact'
CREATE INDEX [IX_FK_SubscriptionDetailPrimaryContact]
ON [dbo].[SubscriptionDetails]
    ([PrimaryContact_Id]);
GO

-- Creating foreign key on [SecondaryContact_Id] in table 'SubscriptionDetails'
ALTER TABLE [dbo].[SubscriptionDetails]
ADD CONSTRAINT [FK_SubscriptionDetailSecondaryContact]
    FOREIGN KEY ([SecondaryContact_Id])
    REFERENCES [dbo].[Contacts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SubscriptionDetailSecondaryContact'
CREATE INDEX [IX_FK_SubscriptionDetailSecondaryContact]
ON [dbo].[SubscriptionDetails]
    ([SecondaryContact_Id]);
GO

-- Creating foreign key on [Contact_Id] in table 'Companies'
ALTER TABLE [dbo].[Companies]
ADD CONSTRAINT [FK_CompanyContact]
    FOREIGN KEY ([Contact_Id])
    REFERENCES [dbo].[Contacts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CompanyContact'
CREATE INDEX [IX_FK_CompanyContact]
ON [dbo].[Companies]
    ([Contact_Id]);
GO

-- Creating foreign key on [SubscriptionDetails_Id] in table 'Companies'
ALTER TABLE [dbo].[Companies]
ADD CONSTRAINT [FK_CompanySubscriptionDetails1]
    FOREIGN KEY ([SubscriptionDetails_Id])
    REFERENCES [dbo].[SubscriptionDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CompanySubscriptionDetails1'
CREATE INDEX [IX_FK_CompanySubscriptionDetails1]
ON [dbo].[Companies]
    ([SubscriptionDetails_Id]);
GO

-- Creating foreign key on [MonthlySubscriptionTemplate_InvoiceDetailsTemplateID] in table 'SubscriptionDetails'
ALTER TABLE [dbo].[SubscriptionDetails]
ADD CONSTRAINT [FK_SubscriptionDetailInvoiceDetailsTemplate]
    FOREIGN KEY ([MonthlySubscriptionTemplate_InvoiceDetailsTemplateID])
    REFERENCES [dbo].[InvoiceDetailsTemplates]
        ([InvoiceDetailsTemplateID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SubscriptionDetailInvoiceDetailsTemplate'
CREATE INDEX [IX_FK_SubscriptionDetailInvoiceDetailsTemplate]
ON [dbo].[SubscriptionDetails]
    ([MonthlySubscriptionTemplate_InvoiceDetailsTemplateID]);
GO

-- Creating foreign key on [MonthlyExtraSMSCharge_InvoiceDetailsTemplateID] in table 'SubscriptionDetails'
ALTER TABLE [dbo].[SubscriptionDetails]
ADD CONSTRAINT [FK_SubscriptionDetailInvoiceDetailsTemplate1]
    FOREIGN KEY ([MonthlyExtraSMSCharge_InvoiceDetailsTemplateID])
    REFERENCES [dbo].[InvoiceDetailsTemplates]
        ([InvoiceDetailsTemplateID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SubscriptionDetailInvoiceDetailsTemplate1'
CREATE INDEX [IX_FK_SubscriptionDetailInvoiceDetailsTemplate1]
ON [dbo].[SubscriptionDetails]
    ([MonthlyExtraSMSCharge_InvoiceDetailsTemplateID]);
GO

-- Creating foreign key on [Devices_Id] in table 'UserDevice'
ALTER TABLE [dbo].[UserDevice]
ADD CONSTRAINT [FK_UserDevice_Device]
    FOREIGN KEY ([Devices_Id])
    REFERENCES [dbo].[Devices]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Users_UserId] in table 'UserDevice'
ALTER TABLE [dbo].[UserDevice]
ADD CONSTRAINT [FK_UserDevice_User]
    FOREIGN KEY ([Users_UserId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserDevice_User'
CREATE INDEX [IX_FK_UserDevice_User]
ON [dbo].[UserDevice]
    ([Users_UserId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------