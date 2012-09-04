SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production; -- choose db

-- Important fields
DECLARE @UserId uniqueidentifier = 'd6e62ef1-e6eb-4110-b828-c18a75dbe51e';

DECLARE @XmppConnId int = (SELECT TOP 1 XmppConnection_Id FROM dbo.Users WHERE UserId = @UserId);
DELETE FROM Memberships WHERE UserId = @UserId;
DELETE FROM UsersForWorkingPoints WHERE Users_UserId = @UserId;
DELETE FROM Users WHERE UserId = @UserId;
DELETE FROM XmppConnections WHERE Id = @XmppConnId;

COMMIT TRAN
