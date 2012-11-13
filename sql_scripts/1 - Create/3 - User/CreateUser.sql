SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production; -- choose db

-- Important fields
DECLARE @RegularUserName varchar(30) = 'supportpl';
DECLARE @RegularPasswordSalt nvarchar(128) = '3uQCvtXaYTMV/ywaBQT25Q==';
DECLARE @ReqularUserPassword nvarchar(128) = 'e9gA9WGlvPtvr6cIOj9zMy+1M9Q=';
DECLARE @XmppUser varchar(30) = 'supportpl@txtfeedback.net';
DECLARE @XmppPassword varchar(30) = '123456';
DECLARE @RegularUserEmail nvarchar(256) = 'supportpl@txtfeedback.net';

-- Less used
DECLARE @CompanyName nvarchar(50) = 'AbMobileApps';
DECLARE @GUID uniqueidentifier = NEWID();
DECLARE @IsApproved bit = 1;
DECLARE @IsLockedOut bit = 0;
DECLARE @isAnonymous bit = 0;
DECLARE @UniversalCounter int = 0;
DECLARE @UniversalDate datetime = GETUTCDATE();
DECLARE @PasswordFormat int = 1;
DECLARE @XmppConnLastId int;

DECLARE @ApplicationId uniqueidentifier = (SELECT TOP 1 ApplicationId FROM dbo.Applications); 
INSERT INTO XmppConnections (XmppUser, XmppPassword) VALUES (@XmppUser, @XmppPassword)

INSERT INTO Users (ApplicationId, UserId, UserName, IsAnonymous,
		LastActivityDate, Company_Name, XmppConnectionXmppUser) 
		VALUES (@ApplicationId, @GUID, @RegularUserName, 
		@isAnonymous, @UniversalDate, @CompanyName, @XmppUser);

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
		 
-- Add roles
DECLARE @ROLE_COMUNICATION_RESPONSIBLE uniqueidentifier;
SET @ROLE_COMUNICATION_RESPONSIBLE = (SELECT TOP 1 RoleId FROM Roles WHERE RoleName = 'ComunicationResponsible' ORDER BY RoleId DESC);
DECLARE @ROLE_WORKING_POINTS_CONFIGURATOR uniqueidentifier;
SET @ROLE_WORKING_POINTS_CONFIGURATOR = (SELECT TOP 1 RoleId FROM Roles WHERE RoleName = 'WorkingPointsConfigurator' ORDER BY RoleId DESC);
INSERT INTO UsersInRoles (UserId, RoleId) VALUES
		(@GUID, @ROLE_COMUNICATION_RESPONSIBLE);
INSERT INTO UsersInRoles (UserId, RoleId) VALUES 
		(@GUID, @ROLE_WORKING_POINTS_CONFIGURATOR);
		
COMMIT TRAN

