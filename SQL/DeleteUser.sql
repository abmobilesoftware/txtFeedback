SET XACT_ABORT ON
BEGIN TRAN

USE txtfeedback_dev;
DECLARE @UserId uniqueidentifier = '7b299e0c-f019-4965-94ac-2d00c4e1f801';

-- local variables
DECLARE @XmppConnId int = (SELECT TOP 1 XmppConnection_Id FROM dbo.Users WHERE UserId = @UserId);

DELETE FROM Memberships WHERE UserId = @UserId;
DELETE FROM UsersForWorkingPoints WHERE Users_UserId = @UserId;
DELETE FROM Users WHERE UserId = @UserId;
DELETE FROM XmppConnections WHERE Id = @XmppConnId;

COMMIT TRAN
