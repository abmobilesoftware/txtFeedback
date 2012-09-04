SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production -- choose db

-- Important fields
DECLARE @Client_TelNumber nvarchar(50) = '16783694507';
DECLARE @Client_Description nvarchar(160) = 'US support';

-- Working point data
DECLARE @WP_Name nvarchar(40) = 'US support';

-- User data
DECLARE @U_RegularUserName varchar(30) = 'supportUS';
DECLARE @U_ReqularUserPassword nvarchar(128) = 'DxriZE+tnITUai9AJvtUSVDCfpc=';
DECLARE @U_RegularPasswordSalt nvarchar(128) = 'NCzaHk6ZyehDlC6aeXxRew==';
DECLARE @U_XmppUser varchar(30) = @Client_TelNumber + '@txtfeedback.net';
DECLARE @U_XmppPassword varchar(30) = '123456';
-- END Important fields

-- Less used
DECLARE @Client_DisplayName nvarchar(50) = 'TxtSupport'; 
DECLARE @WP_Provider nvarchar(50) = 'nexmo';
DECLARE @WP_SmsSent int = 0;
DECLARE @WP_MaxNrOfSmsToSend int = 200;
DECLARE @WP_Description_Text nvarchar(120) = 'WP';
DECLARE @WP_Description nvarchar(160) = @WP_Name + @WP_Description_Text;
DECLARE @U_RegularUserEmail nvarchar(256) = 'support@txtfeedback.net';
DECLARE @U_CompanyName nvarchar(50) = 'AbMobileApps';

-- Don't touch
DECLARE @Client_isSupportClient bit = 1;
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
		VALUES (@Client_TelNumber, @Client_DisplayName, 
		@Client_Description, @Client_isSupportClient);	

-- Add working point
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend) 
		VALUES (@Client_TelNumber, @WP_Description, @WP_Name, 
		@WP_Provider, @WP_SmsSent, @WP_MaxNrOfSmsToSend);

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
