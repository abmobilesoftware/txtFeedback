SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_nexmo

-- Important fields
DECLARE @Client_TelNumber nvarchar(50) = '12152401393';
DECLARE @WP_Name nvarchar(40) = 'Harrods';
DECLARE @Support_TelNumber nvarchar(50) = '16783694507';
DECLARE @WP_ShortID nvarchar(10) = 'dragostest';
DECLARE @WP_XmppSuffix nvarchar(50) = '@compdev.txtfeedback.net';

DECLARE @WP_Support_TelNumber nvarchar(50) = '12898437378';
DECLARE @WP_Support_XmppSuffix nvarchar(50) = '@compdev.txtfeedback.net';
DECLARE @WP_Support_ShortID nvarchar(10) = 'supportca';

-- Less used
DECLARE @WP_Provider nvarchar(50) = 'twilio';
DECLARE @WP_SmsSent int = 0;
DECLARE @WP_MaxNrOfSmsToSend int = 200;
DECLARE @Description_Additional_Text nvarchar(120) = ' WP';
DECLARE @WP_Description nvarchar(160) = @WP_Name + @Description_Additional_Text;
-- Welcome message
DECLARE @WP_WelcomeMessage nvarchar(160) = 'Welcome to TxtFeedback!';
DECLARE @WP_BusyMessage nvarchar(160) = 'Busy';
DECLARE @WP_OutsideOfficeHoursMessage nvarchar(160) = 'Outside of office hours';
DECLARE @WP_ConvIdSupportToWP nvarchar(50);
DECLARE @WP_ConvIdWPToSupport nvarchar(50);

-- Languages for working points
DECLARE @WP_Language_RO nvarchar(10) = 'ro-RO';
DECLARE @WP_Language_US nvarchar(10) = 'en-US';
DECLARE @WP_Language_DE nvarchar(10) = 'de-DE';

DECLARE @TimeReceived datetime = current_timestamp;
DECLARE @Read bit = 0;
--support conversations 
DECLARE @WP_TimeReceived datetime = GETUTCDATE();
DECLARE @ConvIdSupportToWP nvarchar(50) = '';
DECLARE @ConvIdWPToSupport nvarchar(50) = '' ;
DECLARE @Starred bit = 1;
DECLARE @WP_Read bit = 0;
DECLARE @WP_Starred bit = 0;

SET @ConvIdSupportToWP = @Support_TelNumber + '-' + @Client_TelNumber;
SET @ConvIdWPToSupport = @Client_TelNumber + '-' + @Support_TelNumber;
DECLARE @WP_Support_WelcomeMessage nvarchar(160) = 'Welcome to TxtFeedback! Use this chat to clarify any/every question about TxtFeedback';
--Client
DECLARE @Client_isSupportClient bit = 0; 
DECLARE @Support_isSupportClient bit = 1;
DECLARE @isSmsBased_IM bit = 0;
DECLARE @isSmsBased_SMS bit = 1;

DECLARE @Client_Description nvarchar(160) = @WP_Name + ' client';

-- Create clients
IF (SELECT COUNT(*) FROM [dbo].[Clients] WHERE [dbo].[Clients].[TelNumber] = @WP_ShortID) = 0 
INSERT INTO Clients(TelNumber, DisplayName, [Description], isSupportClient)
		VALUES (@WP_ShortID, @WP_ShortID, 
		@Client_Description, @Support_isSupportClient);	

IF (SELECT COUNT(*) FROM [dbo].[Clients] WHERE [dbo].[Clients].[TelNumber] = @WP_Support_ShortID) = 0 
INSERT INTO Clients(TelNumber, DisplayName, [Description], isSupportClient)
	VALUES (@WP_Support_ShortID, @Client_Support_DisplayName,
	 @WP_Support_Description, @Support_IsSupportClient);

-- Create WP
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend,[WelcomeMessage]) 
		VALUES (@TelNumber, @Description, @Name, 
		@Provider, @SmsSent, @MaxNrOfSmsToSend,@WP_Support_WelcomeMessage);

IF (SELECT COUNT(*) FROM [dbo].[WorkingPoints] WHERE [dbo].[WorkingPoints].[TelNumber] = @Client_TelNumber) = 0 
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend, ShortID, XMPPsuffix, WelcomeMessage,
		BusyMessage, OutsideOfficeHoursMessage, [Language]) 
		VALUES (@Client_TelNumber, @WP_Description, @WP_Name, 
		@WP_Provider, @WP_SmsSent, @WP_MaxNrOfSmsToSend, @WP_ShortID, 
		@WP_XmppSuffix, @WP_WelcomeMessage, @WP_BusyMessage,
		@WP_OutsideOfficeHoursMessage, @WP_Language_US);

-- Welcome conversation Support - WP. 
SET @WP_ConvIdSupportToWP = @WP_Support_ShortID + '-' + @WP_ShortID;
SET @WP_ConvIdWPToSupport = @WP_ShortID + '-' + @WP_Support_ShortID;
DECLARE @WP_Support_XmppAddress nvarchar(50);
SET @WP_Support_XmppAddress = @WP_Support_ShortID + @WP_Support_XmppSuffix;
DECLARE @WP_XmppAddress nvarchar(50);
SET @WP_XmppAddress = @WP_ShortID + @WP_XmppSuffix;

INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber, IsSmsBased)
      VALUES (@WP_ConvIdSupportToWP, @WP_Support_WelcomeMessage,
		@WP_Read, @WP_TimeReceived, @WP_Support_ShortID, @WP_Starred, @WP_TimeReceived, 
		@Client_TelNumber, @WP_Support_ShortID, @isSmsBased_IM);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId, XmppConnectionXmppUser)
		VALUES (@WP_Support_XmppAddress, @WP_XmppAddress, @WP_Support_WelcomeMessage,
		@WP_TimeReceived, @WP_Read, @WP_ConvIdSupportToWP, NULL);
		
UPDATE WorkingPoints SET SupportConversation = @WP_ConvIdSupportToWP WHERE TelNumber = @Client_TelNumber;

-- Welcome conversation WP - Support
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber, IsSmsBased) 
      VALUES (@WP_ConvIdWPToSupport, @WP_Support_WelcomeMessage,
		@WP_Read, @WP_TimeReceived, @WP_ShortID, @WP_Starred, @WP_TimeReceived, 
		@WP_Support_TelNumber, @WP_ShortID, @isSmsBased_IM);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId, XmppConnectionXmppUser)
		VALUES (@WP_XmppAddress, @WP_Support_XmppAddress, @WP_Support_WelcomeMessage,
		@WP_TimeReceived, @WP_Read, @WP_ConvIdWPToSupport, NULL);

COMMIT TRAN