SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production -- choose db

-- Important fields
DECLARE @Client_TelNumber nvarchar(50) = '8524569631';
DECLARE @Client_Description nvarchar(160) = 'H&M';
DECLARE @WP_Name nvarchar(40) = 'H&M';
DECLARE @WP_ShortID nvarchar(10) = 'abprod15';

-- Working point data
DECLARE @WP_Support_TelNumber nvarchar(50) = '1597533697';

-- User data
DECLARE @U_RegularUserName varchar(30) = 'dino';
DECLARE @U_ReqularUserPassword nvarchar(128) = '+P4kuRdiAtys31V1FhlI+z/iMxw=';
DECLARE @U_RegularPasswordSalt nvarchar(128) = 'Iv/7pKfCftn7cXKQYhjpdA==';
DECLARE @U_XmppUser varchar(30) = 'txtprod3@txtfeedback.net';
DECLARE @U_XmppPassword varchar(30) = '123456';
DECLARE @U_RegularUserEmail nvarchar(256) = 'user@clientserver.net';

-- END Important fields

-- Less used
DECLARE @WP_Provider nvarchar(50) = 'nexmo';
DECLARE @WP_SmsSent int = 0;
DECLARE @WP_MaxNrOfSmsToSend int = 200;
DECLARE @WP_Description_Additional_Text nvarchar(120) = ' WP';
DECLARE @WP_Description nvarchar(160) = @WP_Name + @WP_Description_Additional_Text;
DECLARE @WP_XmppSuffix nvarchar(50) = '@moderator.txtfeedback.net';
DECLARE @WP_BusyMessage nvarchar(160) = 'Busy';
DECLARE @WP_OutsideOfficeHoursMessage nvarchar(160) = 'Outside of office hours';
DECLARE @WP_Language nvarchar(10) = 'en';
-- Welcome message
DECLARE @WP_WelcomeMessage nvarchar(160) = 'Welcome to TxtFeedback!';
DECLARE @WP_TimeReceived datetime = GETUTCDATE();
DECLARE @WP_Read bit = 0;
DECLARE @WP_ConvIdSupportToWP nvarchar(50);
DECLARE @WP_ConvIdWPToSupport nvarchar(50);
DECLARE @WP_Starred bit = 1;
-- Support WP
DECLARE @WP_Support_Description nvarchar(120)= 'Support WP';
DECLARE @WP_Support_Name nvarchar(40) = 'Support';
DECLARE @WP_Support_Provider nvarchar(50) = 'nexmo';
DECLARE @WP_Support_SentSms int = 0;
DECLARE @WP_Support_MaxNrOfSms int = 200;
DECLARE @WP_Support_ShortID nvarchar(10) = 'abprod1';
DECLARE @WP_Support_WelcomeMessage nvarchar(160) = 'Welcome to TxtFeedback!';
DECLARE @WP_Support_XmppSuffix nvarchar(50) = '@moderator.txtfeedback.net';
DECLARE @WP_Support_BusyMessage nvarchar(160) = 'Busy';
DECLARE @WP_Support_OutsideOfficeHoursMessage nvarchar(160) = 'Outside of office hours';
DECLARE @WP_Support_Language nvarchar(10) = 'en';
-- Company
DECLARE @U_CompanyName nvarchar(50) = 'AbMobileApps';
DECLARE @C_Description nvarchar(max) = 'new company';
DECLARE @C_Address nvarchar(max) = 'company address';
DECLARE @C_NrOfTrainingHoursDelivered int = 12;
-- Subscriptions 
DECLARE @C_Subscription_Type nvarchar(50) = 'Free';
DECLARE @S_NrOfWorkingPoints int = 2;
DECLARE @S_SmsPerWorkingPoint int = 30;
DECLARE @S_NrOfUsers int = 2;
DECLARE @S_NrOfHoursOfTraining nvarchar(max) = 12;
-- Tags
DECLARE @TagDescription nvarchar(200) = 'default tag';
-- English version
DECLARE @TagPositive nvarchar(50) = 'Positive feedback';
DECLARE @TagNegative nvarchar(50) = 'Negative feedback';
-- German version
--DECLARE @TagPositive nvarchar(50) = 'Positives feedback';
--DECLARE @TagNegative nvarchar(50) = 'Negatives feedback';

DECLARE @NegativeTagType NVARCHAR(50) = 'negativeFeedback';
DECLARE @PositiveTagType NVARCHAR(50) = 'positiveFeedback';

-- Don't touch
DECLARE @Client_isSupportClient bit = 0; 
SET @WP_ConvIdSupportToWP = @WP_Support_TelNumber + '-' + @Client_TelNumber;
SET @WP_ConvIdWPToSupport = @Client_TelNumber + '-' + @WP_Support_TelNumber;
-- For support client
DECLARE @Client_Support_Description nvarchar(160) = 'support client';
DECLARE @Client_Support_IsSupport bit = 1;
DECLARE @Client_Support_DisplayName nvarchar(50) = 'Support';
-- For user
DECLARE @U_GUID uniqueidentifier = NEWID();
DECLARE @U_IsApproved bit = 1;
DECLARE @U_IsLockedOut bit = 0;
DECLARE @U_isAnonymous bit = 0;
DECLARE @U_UniversalCounter int = 0;
DECLARE @U_UniversalDate datetime = GETUTCDATE();
DECLARE @U_PasswordFormat int = 1;


IF (SELECT COUNT(*) FROM [dbo].[Clients] WHERE [dbo].[Clients].[TelNumber] = @Client_TelNumber) = 0 
INSERT INTO Clients(TelNumber, DisplayName,
		Description, isSupportClient)
		VALUES (@Client_TelNumber, @Client_TelNumber, 
		@Client_Description, @Client_isSupportClient);	

IF (SELECT COUNT(*) FROM [dbo].[Clients] WHERE [dbo].[Clients].[TelNumber] = @WP_Support_TelNumber) = 0 
INSERT INTO Clients(TelNumber, DisplayName, Description, isSupportClient)
	VALUES (@WP_Support_TelNumber, @Client_Support_DisplayName,
	 @Client_Support_Description, @Client_Support_IsSupport);
	
-- Add working point
IF (SELECT COUNT(*) FROM [dbo].[WorkingPoints] WHERE [dbo].[WorkingPoints].[TelNumber] = @Client_TelNumber) = 0 
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend, ShortID, XMPPsuffix, WelcomeMessage,
		BusyMessage, OutsideOfficeHoursMessage, [Language]) 
		VALUES (@Client_TelNumber, @WP_Description, @WP_Name, 
		@WP_Provider, @WP_SmsSent, @WP_MaxNrOfSmsToSend, @WP_ShortID, 
		@WP_XmppSuffix, @WP_WelcomeMessage, @WP_BusyMessage,
		@WP_OutsideOfficeHoursMessage, @WP_Language);
		
IF (SELECT COUNT(*) FROM [dbo].[WorkingPoints] WHERE [dbo].[WorkingPoints].[TelNumber] = @WP_Support_TelNumber) = 0 
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend, ShortID, XMPPsuffix, WelcomeMessage,
		BusyMessage, OutsideOfficeHoursMessage, [Language]) 
		VALUES (@WP_Support_TelNumber, @WP_Support_Description, @WP_Support_Name, 
		@WP_Support_Provider, @WP_Support_SentSms, @WP_Support_MaxNrOfSms, @WP_Support_ShortID, 
		@WP_Support_XmppSuffix, @WP_Support_WelcomeMessage, @WP_Support_BusyMessage,
		@WP_Support_OutsideOfficeHoursMessage, @WP_Support_Language);

-- Welcome conversation Support - WP. 
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber) VALUES (@WP_ConvIdSupportToWP, @WP_WelcomeMessage,
		@WP_Read, @WP_TimeReceived, @WP_Support_TelNumber, @WP_Starred, @WP_TimeReceived, 
		@Client_TelNumber, @WP_Support_TelNumber);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId, XmppConnectionXmppUser)
		VALUES (@WP_Support_TelNumber, @Client_TelNumber, @WP_WelcomeMessage,
		@WP_TimeReceived, @WP_Read, @WP_ConvIdSupportToWP, NULL);
		
UPDATE WorkingPoints SET SupportConversation = @WP_ConvIdSupportToWP WHERE TelNumber = @Client_TelNumber;

-- Welcome conversation WP - Support
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber) VALUES (@WP_ConvIdWPToSupport, @WP_WelcomeMessage,
		@WP_Read, @WP_TimeReceived, @WP_Support_TelNumber, @WP_Starred, @WP_TimeReceived, 
		@WP_Support_TelNumber, @Client_TelNumber);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId, XmppConnectionXmppUser)
		VALUES (@WP_Support_TelNumber, @Client_TelNumber, @WP_WelcomeMessage,
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
IF (SELECT COUNT(*) FROM [dbo].[Clients] WHERE [dbo].[Clients].[TelNumber] = @WP_Support_TelNumber) = 0 
INSERT INTO Companies (Name, Description, Address, NrOfTrainingHoursDelivered, Subscription_Type)
	 VALUES (@U_CompanyName, @C_Description, @C_Address, @C_NrOfTrainingHoursDelivered, @C_Subscription_Type);
		
IF (SELECT COUNT(*) FROM [dbo].[Tags] WHERE [dbo].[Tags].[Name] = @TagPositive) = 0 
INSERT INTO dbo.Tags(Name, [Description], CompanyName) VALUES (@TagPositive, @TagDescription, @U_CompanyName);
IF (SELECT COUNT(*) FROM [dbo].[Tags] WHERE [dbo].[Tags].[Name] = @TagNegative) = 0 
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
