SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production -- choose db

-- Important fields
DECLARE @Client_TelNumber nvarchar(50) = '12152401393';
DECLARE @Client_Description nvarchar(160) = 'US client';

-- Working point data
DECLARE @WP_Support_TelNumber nvarchar(50) = '16783694507';
DECLARE @WP_Name nvarchar(40) = 'Constitution Hall';

-- User data
DECLARE @U_RegularUserName varchar(30) = 'john';
DECLARE @U_ReqularUserPassword nvarchar(128) = 'xqRZe5/aQM5ikXT2ptw09ujhFo4=';
DECLARE @U_RegularPasswordSalt nvarchar(128) = 'ROdAOWeR0cKGqSUJ7IPw8A==';
DECLARE @U_XmppUser varchar(30) = @Client_TelNumber + '@txtfeedback.net';
DECLARE @U_XmppPassword varchar(30) = '123456';
DECLARE @U_RegularUserEmail nvarchar(256) = 'user@clientserver.net';
-- END Important fields

-- Less used
DECLARE @WP_Provider nvarchar(50) = 'nexmo';
DECLARE @WP_SmsSent int = 0;
DECLARE @WP_MaxNrOfSmsToSend int = 200;
DECLARE @WP_Description_Additional_Text nvarchar(120) = ' WP';
DECLARE @WP_Description nvarchar(160) = @WP_Name + @WP_Description_Additional_Text;
DECLARE @U_CompanyName nvarchar(50) = 'AbMobileApps';
-- Welcome message
DECLARE @WP_WelcomeMessage nvarchar(160) = 'Welcome to TxtFeedback!';
DECLARE @WP_TimeReceived datetime = current_timestamp;
DECLARE @WP_Read bit = 0;
DECLARE @WP_ConvIdSupportToWP nvarchar(50);
DECLARE @WP_ConvIdWPToSupport nvarchar(50);
DECLARE @WP_Starred bit = 1;

-- Don't touch
DECLARE @Client_isSupportClient bit = 0; 
SET @WP_ConvIdSupportToWP = @WP_Support_TelNumber + '-' + @Client_TelNumber;
SET @WP_ConvIdWPToSupport = @Client_TelNumber + '-' + @WP_Support_TelNumber;
-- For user
DECLARE @U_GUID uniqueidentifier = NEWID();
DECLARE @U_IsApproved bit = 1;
DECLARE @U_IsLockedOut bit = 0;
DECLARE @U_isAnonymous bit = 0;
DECLARE @U_UniversalCounter int = 0;
DECLARE @U_UniversalDate datetime = Current_Timestamp;
DECLARE @U_PasswordFormat int = 1;
DECLARE @U_XmppConnLastId int;

-- Add client
INSERT INTO Clients(TelNumber, DisplayName,
		Description, isSupportClient)
		VALUES (@Client_TelNumber, @Client_TelNumber, 
		@Client_Description, @Client_isSupportClient);	

-- Add working point
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend) 
		VALUES (@Client_TelNumber, @WP_Description, @WP_Name, 
		@WP_Provider, @WP_SmsSent, @WP_MaxNrOfSmsToSend);

-- Welcome conversation Support - WP. 
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber) VALUES (@WP_ConvIdSupportToWP, @WP_WelcomeMessage,
		@WP_Read, @WP_TimeReceived, @WP_Support_TelNumber, @WP_Starred, @WP_TimeReceived, 
		@Client_TelNumber, @WP_Support_TelNumber);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId)
		VALUES (@WP_Support_TelNumber, @Client_TelNumber, @WP_WelcomeMessage,
		@WP_TimeReceived, @WP_Read, @WP_ConvIdSupportToWP);

UPDATE WorkingPoints SET SupportConversation = @WP_ConvIdSupportToWP WHERE TelNumber = @Client_TelNumber;

-- Welcome conversation WP - Support
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber) VALUES (@WP_ConvIdWPToSupport, @WP_WelcomeMessage,
		@WP_Read, @WP_TimeReceived, @WP_Support_TelNumber, @WP_Starred, @WP_TimeReceived, 
		@WP_Support_TelNumber, @Client_TelNumber);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId)
		VALUES (@WP_Support_TelNumber, @Client_TelNumber, @WP_WelcomeMessage,
		@WP_TimeReceived, @WP_Read, @WP_ConvIdWPToSupport);

-- Add user
IF (SELECT COUNT(*) FROM dbo.XmppConnections) > 0
	 SET @U_XmppConnLastId = (SELECT TOP 1 Id FROM dbo.XmppConnections ORDER BY Id DESC);
ELSE 
	 SET @U_XmppConnLastId = 0;
DECLARE @U_ApplicationId uniqueidentifier = (SELECT TOP 1 ApplicationId FROM dbo.Applications); 

SELECT @U_XmppConnLastId = @U_XmppConnLastId + 1;
INSERT INTO XmppConnections (XmppUser, XmppPassword, Id) VALUES (@U_XmppUser, @U_XmppPassword, @U_XmppConnLastId)

INSERT INTO Users (ApplicationId, UserId, UserName, IsAnonymous,
		LastActivityDate, Company_Name, XmppConnection_Id) 
		VALUES (@U_ApplicationId, @U_GUID, @U_RegularUserName, 
		@U_isAnonymous, @U_UniversalDate, @U_CompanyName, @U_XmppConnLastId);

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
		
COMMIT TRAN
