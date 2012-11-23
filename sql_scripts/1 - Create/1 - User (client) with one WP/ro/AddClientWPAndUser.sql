SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_nexmo -- choose db 
-- Script for Romania

-- Client working point
DECLARE @Client_TelNumber nvarchar(50) = '9999912346';
DECLARE @WP_ShortID nvarchar(10) = 'active2';
DECLARE @WP_Name nvarchar(40) = 'Active 2';
DECLARE @Client_Description nvarchar(160) = 'Active romania';
DECLARE @WP_XmppSuffix nvarchar(50) = '@compdev.txtfeedback.net';
DECLARE @WP_WelcomeMessage nvarchar(160) = 'Bine ati venit!';

-- Company
DECLARE @U_CompanyName nvarchar(50) = 'Active'; -- details in less used section

-- Subscription
DECLARE @C_Subscription_Type nvarchar(50) = 'Free'; -- details in less used section

-- Support working point
DECLARE @WP_Support_TelNumber nvarchar(50) = '2220000100';
DECLARE @WP_Support_ShortID nvarchar(10) = 'supportro';
DECLARE @WP_Support_Name nvarchar(40) = 'RO support';
DECLARE @WP_Support_Description nvarchar(120)= 'Support for Romania';
DECLARE @WP_Support_XmppSuffix nvarchar(50) = '@compdev.txtfeedback.net';
DECLARE @WP_Support_WelcomeMessage nvarchar(160) = 'Bine ati venit! Pentru orice fel de intrebare legata de modul de functionare al sistemului nostru folositi acest chat.';

-- User data
DECLARE @U_RegularUserName varchar(30) = 'active2';
DECLARE @U_ReqularUserPassword nvarchar(128) = '4fynfp1TLWGBGDGnAzYskrKImVk=';
DECLARE @U_RegularPasswordSalt nvarchar(128) = '48z4v6x3KgaBBpFMxUaAIg==';
DECLARE @U_XmppUser varchar(30) = 'activero2@txtfeedback.net';
DECLARE @U_XmppPassword varchar(30) = 'c(/qU~C*o';
DECLARE @U_RegularUserEmail nvarchar(256) = 'no@email.ro';
-- END Important fields

-- Less used
DECLARE @WP_Provider nvarchar(50) = 'nexmo';
DECLARE @WP_SmsSent int = 0;
DECLARE @WP_MaxNrOfSmsToSend int = 200;
DECLARE @WP_Description_Additional_Text nvarchar(120) = ' WP';
DECLARE @WP_Description nvarchar(160) = @WP_Name + @WP_Description_Additional_Text;
DECLARE @WP_BusyMessage nvarchar(160) = 'Busy';
DECLARE @WP_OutsideOfficeHoursMessage nvarchar(160) = 'In afara orelor de program. Va rugam, reveniti!';
-- Welcome message
DECLARE @WP_TimeReceived datetime = GETUTCDATE();
DECLARE @WP_Read bit = 0;
DECLARE @WP_ConvIdSupportToWP nvarchar(50);
DECLARE @WP_ConvIdWPToSupport nvarchar(50);
DECLARE @WP_Starred bit = 1;
-- Support WP
DECLARE @WP_Support_Provider nvarchar(50) = 'nexmo';
DECLARE @WP_Support_SentSms int = 0;
DECLARE @WP_Support_MaxNrOfSms int = 200;
DECLARE @WP_Support_BusyMessage nvarchar(160) = 'Busy';
DECLARE @WP_Support_OutsideOfficeHoursMessage nvarchar(160) = 'In afara orelor de program. Va rugam, reveniti!';
-- Company
DECLARE @C_Description nvarchar(max) = 'new company';
DECLARE @C_Address nvarchar(max) = 'company address';
DECLARE @C_NrOfTrainingHoursDelivered int = 12;
-- Subscriptions 
DECLARE @S_NrOfWorkingPoints int = 2;
DECLARE @S_SmsPerWorkingPoint int = 30;
DECLARE @S_NrOfUsers int = 2;
DECLARE @S_NrOfHoursOfTraining nvarchar(max) = 12;

-- Tags
DECLARE @TagDescription nvarchar(200) = 'default tag';
-- English version
DECLARE @TagPositive nvarchar(50) = 'Feedback pozitiv';
DECLARE @TagNegative nvarchar(50) = 'Feedback negativ';
-- German version
--DECLARE @TagPositive nvarchar(50) = 'Positives feedback';
--DECLARE @TagNegative nvarchar(50) = 'Negatives feedback';

DECLARE @NegativeTagType NVARCHAR(50) = 'negativeFeedback';
DECLARE @PositiveTagType NVARCHAR(50) = 'positiveFeedback';
-- End tags

-- Don't touch
DECLARE @Client_isSupportClient bit = 0; 
DECLARE @Support_isSupportClient bit = 1;
DECLARE @isSmsBased_IM bit = 0;
DECLARE @isSmsBased_SMS bit = 1;

DECLARE @WP_XmppAddress nvarchar(50);
SET @WP_XmppAddress = @WP_ShortID + @WP_XmppSuffix;
DECLARE @WP_Support_XmppAddress nvarchar(50);
SET @WP_Support_XmppAddress = @WP_Support_ShortID + @WP_Support_XmppSuffix;

SET @WP_ConvIdSupportToWP = @WP_Support_ShortID + '-' + @WP_ShortID;
SET @WP_ConvIdWPToSupport = @WP_ShortID + '-' + @WP_Support_ShortID;
-- For support client
DECLARE @Client_Support_Description nvarchar(160) = 'support client';
DECLARE @Client_Support_IsSupport bit = 1;
DECLARE @Client_Support_DisplayName nvarchar(50) = 'TxtSupport';
-- For user
DECLARE @U_GUID uniqueidentifier = NEWID();
DECLARE @U_IsApproved bit = 1;
DECLARE @U_IsLockedOut bit = 0;
DECLARE @U_isAnonymous bit = 0;
DECLARE @U_UniversalCounter int = 0;
DECLARE @U_UniversalDate datetime = GETUTCDATE();
DECLARE @U_PasswordFormat int = 1;
-- Languages for working points
DECLARE @WP_Language_RO nvarchar(10) = 'ro-RO';
DECLARE @WP_Language_US nvarchar(10) = 'en-US';
DECLARE @WP_Language_DE nvarchar(10) = 'de-DE';


-- Create clients
IF (SELECT COUNT(*) FROM [dbo].[Clients] WHERE [dbo].[Clients].[TelNumber] = @WP_ShortID) = 0 
INSERT INTO Clients(TelNumber, DisplayName,
		Description, isSupportClient)
		VALUES (@WP_ShortID, @WP_ShortID, 
		@Client_Description, @Support_isSupportClient);	

IF (SELECT COUNT(*) FROM [dbo].[Clients] WHERE [dbo].[Clients].[TelNumber] = @WP_Support_ShortID) = 0 
INSERT INTO Clients(TelNumber, DisplayName, Description, isSupportClient)
	VALUES (@WP_Support_ShortID, @Client_Support_DisplayName,
	 @WP_Support_Description, @Support_IsSupportClient);
	
-- Add working point
IF (SELECT COUNT(*) FROM [dbo].[WorkingPoints] WHERE [dbo].[WorkingPoints].[TelNumber] = @Client_TelNumber) = 0 
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend, ShortID, XMPPsuffix, WelcomeMessage,
		BusyMessage, OutsideOfficeHoursMessage, [Language]) 
		VALUES (@Client_TelNumber, @WP_Description, @WP_Name, 
		@WP_Provider, @WP_SmsSent, @WP_MaxNrOfSmsToSend, @WP_ShortID, 
		@WP_XmppSuffix, @WP_WelcomeMessage, @WP_BusyMessage,
		@WP_OutsideOfficeHoursMessage, @WP_Language_RO);
		
IF (SELECT COUNT(*) FROM [dbo].[WorkingPoints] WHERE [dbo].[WorkingPoints].[TelNumber] = @WP_Support_TelNumber) = 0 
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend, ShortID, XMPPsuffix, WelcomeMessage,
		BusyMessage, OutsideOfficeHoursMessage, [Language]) 
		VALUES (@WP_Support_TelNumber, @WP_Support_Description, @WP_Support_Name, 
		@WP_Support_Provider, @WP_Support_SentSms, @WP_Support_MaxNrOfSms, @WP_Support_ShortID, 
		@WP_Support_XmppSuffix, @WP_Support_WelcomeMessage, @WP_Support_BusyMessage,
		@WP_Support_OutsideOfficeHoursMessage, @WP_Language_RO);

-- Welcome conversation Support - WP. 
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber, IsSmsBased) VALUES (@WP_ConvIdSupportToWP, @WP_Support_WelcomeMessage,
		@WP_Read, @WP_TimeReceived, @WP_Support_ShortID, @WP_Starred, @WP_TimeReceived, 
		@Client_TelNumber, @WP_Support_ShortID, @isSmsBased_IM);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId, XmppConnectionXmppUser)
		VALUES (@WP_Support_XmppAddress, @WP_XmppAddress, @WP_Support_WelcomeMessage,
		@WP_TimeReceived, @WP_Read, @WP_ConvIdSupportToWP, NULL);
		
UPDATE WorkingPoints SET SupportConversation = @WP_ConvIdSupportToWP WHERE TelNumber = @Client_TelNumber;

-- Welcome conversation WP - Support
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber, IsSmsBased) VALUES (@WP_ConvIdWPToSupport, @WP_Support_WelcomeMessage,
		@WP_Read, @WP_TimeReceived, @WP_ShortID, @WP_Starred, @WP_TimeReceived, 
		@WP_Support_TelNumber, @WP_ShortID, @isSmsBased_IM);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId, XmppConnectionXmppUser)
		VALUES (@WP_XmppAddress, @WP_Support_XmppAddress, @WP_Support_WelcomeMessage,
		@WP_TimeReceived, @WP_Read, @WP_ConvIdWPToSupport, NULL);

DECLARE @U_ApplicationId uniqueidentifier = (SELECT TOP 1 ApplicationId FROM dbo.Applications); 

-- Check if the connection exists
IF (SELECT COUNT(*) FROM [dbo].[XmppConnections] WHERE [dbo].[XmppConnections].[XmppUser] = @U_XmppUser) = 0 
INSERT INTO XmppConnections (XmppUser, XmppPassword) VALUES (@U_XmppUser, @U_XmppPassword)

-- Check for subscriptions
IF (SELECT COUNT(*) FROM [dbo].[Subscriptions] WHERE [dbo].[Subscriptions].[Type] = @C_Subscription_Type) = 0 
INSERT INTO Subscriptions (Type, NrOfWorkingPoints, SmsPerWorkingPoint, NrOfUsers, NrOfHoursOfTraining)
	 VALUES (@C_Subscription_Type, @S_NrOfWorkingPoints, @S_SmsPerWorkingPoint, @S_NrOfUsers, @S_NrOfHoursOfTraining);

-- Check the company
IF (SELECT COUNT(*) FROM [dbo].[Companies] WHERE [dbo].[Companies].[Name] = @U_CompanyName) = 0 
INSERT INTO Companies (Name, Description, Address, NrOfTrainingHoursDelivered, Subscription_Type)
	 VALUES (@U_CompanyName, @C_Description, @C_Address, @C_NrOfTrainingHoursDelivered, @C_Subscription_Type);
		
IF (SELECT COUNT(*) FROM [dbo].[Tags] WHERE ([dbo].[Tags].[Name] = @TagPositive) AND ([dbo].[Tags].[CompanyName] = @U_CompanyName) ) = 0 
INSERT INTO dbo.Tags(Name, [Description], CompanyName) VALUES (@TagPositive, @TagDescription, @U_CompanyName);
IF (SELECT COUNT(*) FROM [dbo].[Tags] WHERE ([dbo].[Tags].[Name] = @TagNegative) AND ([dbo].[Tags].[CompanyName] = @U_CompanyName) ) = 0 
INSERT INTO dbo.Tags(Name, [Description], CompanyName) VALUES (@TagNegative, @TagDescription, @U_CompanyName);

IF (SELECT COUNT(*) FROM [dbo].[TagTagTypes] WHERE ([dbo].[TagTagTypes].[TagName] = @TagPositive) AND ([dbo].[TagTagTypes].[TagCompanyName] = @U_CompanyName)) = 0 
INSERT INTO dbo.TagTagTypes
        ( IsDefault ,
          TagName ,
          TagCompanyName ,
          TagTypeType
        )
VALUES  ( 1 , -- IsDefault - bit
          @TagPositive , -- TagName - nvarchar(50)
          @U_CompanyName , -- TagCompanyName - nvarchar(50)
          @PositiveTagType  -- TagTypeType - nvarchar(50)
        )

IF (SELECT COUNT(*) FROM [dbo].[TagTagTypes] WHERE ([dbo].[TagTagTypes].[TagName] = @TagNegative) AND ([dbo].[TagTagTypes].[TagCompanyName] = @U_CompanyName)) = 0 
INSERT INTO dbo.TagTagTypes
        ( IsDefault ,
          TagName ,
          TagCompanyName ,
          TagTypeType
        )
VALUES  ( 1 , -- IsDefault - bit
          @TagNegative , -- TagName - nvarchar(50)
          @U_CompanyName , -- TagCompanyName - nvarchar(50)
          @NegativeTagType -- TagTypeType - nvarchar(50)
        )        

INSERT INTO Users (ApplicationId, UserId, UserName, IsAnonymous,
		LastActivityDate, Company_Name, XmppConnectionXmppUser) 
		VALUES (@U_ApplicationId, @U_GUID, @U_RegularUserName, 
		@U_isAnonymous, @U_UniversalDate, @U_CompanyName, @U_XmppUser);

INSERT INTO Memberships (ApplicationId, UserId, [Password],
		 PasswordFormat, PasswordSalt, Email, IsApproved,
		 IsLockedOut, CreateDate, LastLoginDate,
		 LastPasswordChangedDate, LastLockoutDate,
		 FailedPasswordAnswerAttemptWindowsStart, 
		 FailedPasswordAttemptCount,
		 FailedPasswordAnswerAttemptCount,
		 FailedPasswordAttemptWindowStart) 
		 VALUES (@U_ApplicationId, @U_GUID, @U_ReqularUserPassword, @U_PasswordFormat,
		 @U_RegularPasswordSalt, @U_RegularUserEmail, @U_IsApproved, @U_IsLockedOut,
		 @U_UniversalDate, @U_UniversalDate, @U_UniversalDate,@U_UniversalDate,
		 @U_UniversalDate, @U_UniversalCounter, @U_UniversalCounter, @U_UniversalDate);
		
INSERT INTO UsersForWorkingPoints (Users_UserId, WorkingPoints_TelNumber) VALUES
		(@U_GUID, @Client_TelNumber);
		
-- Add roles
DECLARE @ROLE_COMUNICATION_RESPONSIBLE uniqueidentifier;
SET @ROLE_COMUNICATION_RESPONSIBLE = (SELECT TOP 1 RoleId FROM Roles WHERE RoleName = 'ComunicationResponsible' ORDER BY RoleId DESC);
DECLARE @ROLE_WORKING_POINTS_CONFIGURATOR uniqueidentifier;
SET @ROLE_WORKING_POINTS_CONFIGURATOR = (SELECT TOP 1 RoleId FROM Roles WHERE RoleName = 'WorkingPointsConfigurator' ORDER BY RoleId DESC);
INSERT INTO UsersInRoles (UserId, RoleId) VALUES
		(@U_GUID, @ROLE_COMUNICATION_RESPONSIBLE);
INSERT INTO UsersInRoles (UserId, RoleId) VALUES 
		(@U_GUID, @ROLE_WORKING_POINTS_CONFIGURATOR);
		

		
COMMIT TRAN
