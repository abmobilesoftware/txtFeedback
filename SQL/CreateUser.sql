SET XACT_ABORT ON
BEGIN TRAN

USE txtfeedback_dev;
DECLARE @XmppUser varchar(30) = 'supportCA@txtfeedback.net';
DECLARE @XmppPassword varchar(30) = 'cft67ujm!';
DECLARE @RegularUserName varchar(30) = 'supportCA';
DECLARE @ReqularUserPassword nvarchar(128) = 'mWENrt2AOYTVvUQpcFFsaX/DCME=';
DECLARE @RegularPasswordSalt nvarchar(128) = 'z06rt5WhAen/Jj/xRheGJw==';

DECLARE @RegularUserEmail nvarchar(256) = 'supportCA@txtfeedback.net';
DECLARE @CompanyName nvarchar(50) = 'AbMobileSupport';
DECLARE @WorkingPointTelNumber nvarchar(50) = '12898437378';

DECLARE @GUID uniqueidentifier = NEWID();
DECLARE @IsApproved bit = 1;
DECLARE @IsLockedOut bit = 0;
DECLARE @isAnonymous bit = 0;
DECLARE @UniversalCounter int = 0;
DECLARE @UniversalDate datetime = Current_Timestamp;
DECLARE @PasswordFormat int = 1;

-- local variables
DECLARE @XmppConnLastId int = (SELECT TOP 1 Id FROM dbo.XmppConnections ORDER BY Id DESC);
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
		
INSERT INTO UsersForWorkingPoints (Users_UserId, WorkingPoints_TelNumber) VALUES
		(@GUID, @WorkingPointTelNumber);	
		
COMMIT TRAN
