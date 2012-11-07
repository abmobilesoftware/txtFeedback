SET XACT_ABORT ON
BEGIN TRANSACTION
USE txtfeedback_production;

 -- Remove foreign key and index constraint
ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_UserMessages];
ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserXmppConnection];
DROP INDEX [dbo].[Messages].[IX_FK_UserMessages];
 -- Remove foreign key and index constraint for column XmppConnection_Id from User ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserXmppConnection];
DROP INDEX [dbo].[Users].[IX_FK_UserXmppConnection];
 -- Remove primary key constraint for column Id from XmppConnections
ALTER TABLE [dbo].[XmppConnections] DROP CONSTRAINT [PK_XmppConnections];
 -- Alter type of the XmppUser column so it can be used as an index, nvarchar(max) -> nvarchar(100) and NOT NULL;
ALTER TABLE [dbo].[XmppConnections] ALTER COLUMN [XmppUser] NVARCHAR(100) NOT NULL;
 -- Make XmppUser the primary key of the XmppConnections table
ALTER TABLE [dbo].[XmppConnections] ADD CONSTRAINT [PK_XmppConnections] PRIMARY KEY CLUSTERED ([XmppUser] ASC);
-- Add a new column in Users, to link with XmppConnections
ALTER TABLE [dbo].[Users] ADD [XmppConnectionXmppUser] NVARCHAR(100)  NOT NULL DEFAULT 'supportUK@txtfeedback.net';
-- Add a new column in Messages to link with XmppConnections
ALTER TABLE [dbo].[Messages] ADD [XmppConnectionXmppUser] NVARCHAR(100)  NULL;
ALTER TABLE [dbo].[Messages] ADD [IsSmsBased] bit NOT NULL DEFAULT 1;
ALTER TABLE [dbo].[Conversations] ADD [IsSmsBased] bit NOT NULL DEFAULT 1;


COMMIT TRAN

