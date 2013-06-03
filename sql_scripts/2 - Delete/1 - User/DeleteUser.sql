SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production; -- choose db

-- Important fields
DECLARE @UserId uniqueidentifier = 'ea1dff45-d1e4-4ad5-ae4a-431db5c66036';

DECLARE @XmppConnId nvarchar = (SELECT TOP 1 XmppConnectionXmppUser FROM dbo.Users WHERE UserId = @UserId);
DELETE FROM Memberships WHERE UserId = @UserId;
DELETE FROM UsersForWorkingPoints WHERE Users_UserId = @UserId;
DELETE FROM UsersInRoles WHERE UserId = @UserId;
DELETE FROM Users WHERE UserId = @UserId;
DELETE FROM XmppConnections WHERE XmppUser = @XmppConnId;

COMMIT TRAN
