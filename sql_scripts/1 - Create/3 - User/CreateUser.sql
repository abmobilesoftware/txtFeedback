SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production; -- choose db

-- Important fields
DECLARE @RegularUserName varchar(30) = 'john';
DECLARE @RegularPasswordSalt nvarchar(128) = 'ys0i0yEGycfLXwKF0lmkBw==';
DECLARE @ReqularUserPassword nvarchar(128) = 'H6ZpmRmX0s6P3+pHhPPJdbElEFE=';
DECLARE @XmppUser varchar(30) = @RegularUserName + '@txtfeedback.net';
DECLARE @XmppPassword varchar(30) = '123456';
DECLARE @RegularUserEmail nvarchar(256) = 'john@clientserver.net';

-- Less used
DECLARE @CompanyName nvarchar(50) = 'AbMobileApps';
DECLARE @GUID uniqueidentifier = NEWID();
DECLARE @IsApproved bit = 1;
DECLARE @IsLockedOut bit = 0;
DECLARE @isAnonymous bit = 0;
DECLARE @UniversalCounter int = 0;
DECLARE @UniversalDate datetime = Current_Timestamp;
DECLARE @PasswordFormat int = 1;
DECLARE @XmppConnLastId int;

IF (SELECT COUNT(*) FROM dbo.XmppConnections) > 0
	 SET @XmppConnLastId = (SELECT TOP 1 Id FROM dbo.XmppConnections ORDER BY Id DESC);
ELSE 
	 SET @XmppConnLastId = 0;
DECLARE @ApplicationId uniqueidentifier = (SELECT TOP 1 ApplicationId FROM dbo.Applications); 
SELECT @XmppConnLastId = @XmppConnLastId + 1;
INSERT INTO XmppConnections (XmppUser, XmppPassword, Id) VALUES (@XmppUser, @XmppPassword, @XmppConnLastId)

INSERT INTO Users (ApplicationId, UserId, UserName, IsAnonymous,
		LastActivityDate, Company_Name, XmppConnection_Id) 
		VALUES (@ApplicationId, @GUID, @RegularUserName, 
		@isAnonymous, @UniversalDate, @CompanyName, @XmppConnLastId);

INSERT INTO Memberships (ApplicationId, UserId, [Password],
		 PasswordFormat, PasswordSalt, Email, IsApproved,
		 IsLockedOut, CreateDate, LastLoginDate,
		 LastPasswordChangedDate, LastLockoutDate,
		 FailedPasswordAnswerAttemptWindowsStart, 
		 FailedPasswordAttemptCount,
		 FailedPasswordAnswerAttemptCount,
		 FailedPasswordAttemptWindowStart) 
		 VALUES (@ApplicationId, @GUID, @ReqularUserPassword, @PasswordFormat,
		 @RegularPasswordSalt, @RegularUserEmail, @IsApproved, @IsLockedOut,
		 @UniversalDate, @UniversalDate, @UniversalDate,@UniversalDate,
		 @UniversalDate, @UniversalCounter, @UniversalCounter, @UniversalDate);
		
COMMIT TRAN

