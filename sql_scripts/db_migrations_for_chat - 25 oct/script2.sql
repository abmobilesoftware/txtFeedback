SET XACT_ABORT ON
BEGIN TRANSACTION
USE txtfeedback_production;

 -- Migrarea datelor
 -- 1 Pe baza xmpp_id-ului populez coloana xmpp_user din tablea Users
	UPDATE [dbo].[Users] SET [dbo].[Users].[XmppConnectionXmppUser] = (SELECT [dbo].[XmppConnections].[XmppUser] FROM [dbo].[XmppConnections] WHERE [dbo].[XmppConnections].[Id] = [dbo].[Users].[XmppConnection_Id]); 
 
 -- 2 Pe baza user_id-ului populez coloana xmpp_user din tabela Messages
	UPDATE [dbo].[Messages] SET [dbo].[Messages].[XmppConnectionXmppUser] = (SELECT [dbo].[Users].[XmppConnectionXmppUser] FROM [dbo].[Users] WHERE [dbo].[Users].[UserId] = [dbo].[Messages].[UserUserId]);
	-- 3 Remove column from Users
	ALTER TABLE [dbo].[Users] DROP COLUMN [XmppConnection_Id];
	-- 4 Remove column from XmppConnections
	ALTER TABLE [dbo].[XmppConnections] DROP COLUMN [Id];
	-- 5 Remove column from Messages
	ALTER TABLE [dbo].[Messages] DROP COLUMN [UserUserId];  
-- Creating foreign key on [XmppConnectionXmppUser] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserXmppConnection]
    FOREIGN KEY ([XmppConnectionXmppUser])
    REFERENCES [dbo].[XmppConnections]
        ([XmppUser])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
-- Creating non-clustered index for FOREIGN KEY 'FK_UserXmppConnection'
CREATE INDEX [IX_FK_UserXmppConnection]
ON [dbo].[Users]
    ([XmppConnectionXmppUser]);
-- Creating foreign key on [XmppConnectionXmppUser] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_MessageXmppConnection]
    FOREIGN KEY ([XmppConnectionXmppUser])
    REFERENCES [dbo].[XmppConnections]
        ([XmppUser])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
-- Creating non-clustered index for FOREIGN KEY 'FK_MessageXmppConnection'
CREATE INDEX [IX_FK_MessageXmppConnection]
ON [dbo].[Messages]
    ([XmppConnectionXmppUser]);


ALTER TABLE [dbo].[WorkingPoints]  ADD [ShortID] nvarchar(10)  NOT NULL  DEFAULT 'abmo3'
ALTER TABLE [dbo].[WorkingPoints]  ADD [XMPPsuffix] nvarchar(50) NOT NULL  DEFAULT '@moderator.txtfeedback.net'
ALTER TABLE [dbo].[WorkingPoints]  ADD [WelcomeMessage] nvarchar(160)  NULL
ALTER TABLE [dbo].[WorkingPoints] ADD  [BusyMessage] nvarchar(160) NULL
ALTER TABLE [dbo].[WorkingPoints] ADD  [OutsideOfficeHoursMessage] nvarchar(160)  NULL
ALTER TABLE [dbo].[WorkingPoints] ADD  [Language] nvarchar(10)  NOT NULL DEFAULT 'en-US'

CREATE INDEX [IX_ShortID_WorkingPoint]
ON [dbo].[WorkingPOints]
    ([ShortID]);

COMMIT TRAN



