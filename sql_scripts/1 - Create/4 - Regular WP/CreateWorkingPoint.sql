SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production

-- Important fields
DECLARE @TelNumber nvarchar(50) = '12152401393';
DECLARE @Name nvarchar(40) = 'Harrods';
DECLARE @Support_TelNumber nvarchar(50) = '16783694507';

-- Less used
DECLARE @Provider nvarchar(50) = 'twilio';
DECLARE @SmsSent int = 0;
DECLARE @MaxNrOfSmsToSend int = 200;
DECLARE @Description_Additional_Text nvarchar(120) = ' WP';
DECLARE @Description nvarchar(160) = @Name + @Description_Additional_Text;
-- Welcome message
DECLARE @WelcomeMessage nvarchar(160) = 'Welcome to TxtFeedback!';
DECLARE @TimeReceived datetime = current_timestamp;
DECLARE @Read bit = 0;
DECLARE @ConvIdSupportToWP nvarchar(50);
DECLARE @ConvIdWPToSupport nvarchar(50);
DECLARE @Starred bit = 1;
SET @ConvIdSupportToWP = @Support_TelNumber + '-' + @TelNumber;
SET @ConvIdWPToSupport = @TelNumber + '-' + @Support_TelNumber;
--Client
DECLARE @Client_isSupportClient bit = 0;
DECLARE @Client_Description nvarchar(160) = @Name + ' client';

INSERT INTO Clients(TelNumber, DisplayName,
		Description, isSupportClient)
		VALUES (@TelNumber, @TelNumber, 
		@Client_Description, @Client_isSupportClient);	
		
-- Create WP
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend) 
		VALUES (@TelNumber, @Description, @Name, 
		@Provider, @SmsSent, @MaxNrOfSmsToSend);

-- Welcome conversation Support - WP. 
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber) VALUES (@ConvIdSupportToWP, @WelcomeMessage,
		@Read, @TimeReceived, @Support_TelNumber, @Starred, @TimeReceived, 
		@TelNumber, @Support_TelNumber);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId)
		VALUES (@Support_TelNumber, @TelNumber, @WelcomeMessage,
		@TimeReceived, @Read, @ConvIdSupportToWP);

UPDATE WorkingPoints SET SupportConversation = @ConvIdSupportToWP WHERE TelNumber = @TelNumber;

-- Welcome conversation WP - Support
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber) VALUES (@ConvIdWPToSupport, @WelcomeMessage,
		@Read, @TimeReceived, @Support_TelNumber, @Starred, @TimeReceived, 
		@Support_TelNumber, @TelNumber);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId)
		VALUES (@Support_TelNumber, @TelNumber, @WelcomeMessage,
		@TimeReceived, @Read, @ConvIdWPToSupport);

COMMIT TRAN