DECLARE @TransactionName varchar(30) = 'CreateUser';

BEGIN TRANSACTION @TransactionName
USE txtfeedback_dev;
DECLARE @XmppUser varchar(30) = 'supportCA@txtfeedback.net';
DECLARE @XmppPassword varchar(30) = '123456';
DECLARE @RegularUserName varchar(30) = 'supportCA';
DECLARE @ReqularUserPassword nvarchar(255) = '123456';
DECLARE @RegularUserEmail nvarchar(256) = 'support@txtfeedback.net';
DECLARE @CompanyName nvarchar(50) = 'AbMobileSupport';
DECLARE @WorkingPointTelNumber nvarchar(5) = '441157070678';
DECLARE @PasswordSalt nvarchar(128) = '0xs4a74adacxz14212@sa';
DECLARE @RegularUserPasswordHashed nvarchar(128);
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

--ENCRYPTING THE PASSWORD - here or on server side
SELECT @RegularUserPasswordHashed = Cast((HASHBYTES('SHA1', 
		Cast(@PasswordSalt As nvarchar) + Cast(@ReqularUserPassword As nvarchar))) As NVARCHAR(128));

INSERT INTO Users (ApplicationId, UserId, UserName, IsAnonymous,
		LastActivityDate, Company_Name, XmppConnection_Id) 
		VALUES (@ApplicationId, @GUID, @RegularUserName, 
		@isAnonymous, @UniversalDate, @CompanyName, @XmppConnLastId);

INSERT INTO Memberships (ApplicationId, UserId, Password,
		 PasswordFormat, PasswordSalt, Email, IsApproved,
		 IsLockedOut, CreateDate, LastLoginDate,
		 LastPasswordChangedDate, LastLockoutDate,
		 FailedPasswordAnswerAttemptWindowsStart, 
		 FailedPasswordAttemptCount,
		 FailedPasswordAnswerAttemptCount,
		 FailedPasswordAttemptWindowStart) 
		 VALUES (@ApplicationId, @GUID, @RegularUserPasswordHashed, @PasswordFormat,
		 @PasswordSalt, @RegularUserEmail, @IsApproved, @IsLockedOut,
		 @UniversalDate, @UniversalDate, @UniversalDate,@UniversalDate,
		 @UniversalDate, @UniversalCounter, @UniversalCounter, @UniversalDate);
		
INSERT INTO UsersForWorkingPoints (Users_UserId, WorkingPoints_TelNumber) VALUES 
		(@GUID, @WorkingPointTelNumber);		 


COMMIT TRANSACTION @TransactionName;
