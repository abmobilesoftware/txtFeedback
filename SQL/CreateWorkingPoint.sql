SET XACT_ABORT ON
BEGIN TRAN

USE txtfeedback_dev;
DECLARE @TelNumber nvarchar(50) = '12898437378';
DECLARE @Description nvarchar(160) = 'CA Support WorkingPoint';
DECLARE @Name nvarchar(40) = 'CA Support';
DECLARE @Provider nvarchar(50) = 'nexmo';
DECLARE @Support_TelNumber nvarchar(50) = '12898437378';
DECLARE @SmsSent int = 0;
DECLARE @MaxNrOfSmsToSend int = 200;
DECLARE @WelcomeMessage nvarchar(160) = 'Welcome to TxtFeedback!';
DECLARE @TimeReceived datetime = current_timestamp;
DECLARE @Read bit = 0;
DECLARE @ConversationId nvarchar(50);
DECLARE @Starred bit = 1;
SET @ConversationId = @Support_TelNumber + '-' + @TelNumber;

-- Create WP
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, Support_TelNumber,
		SentSms, MaxNrOfSmsToSend) 
		VALUES (@TelNumber, @Description, @Name, 
		@Provider, @Support_TelNumber,
		@SmsSent, @MaxNrOfSmsToSend);

-- Create Welcome conversation and attach to WP
INSERT INTO Conversations (ConvId, [Text], [Read], TimeUpdated,
		[From], Starred, StartTime, WorkingPoint_TelNumber, 
		Client_TelNumber) VALUES (@ConversationId, @WelcomeMessage,
		@Read, @TimeReceived, @Support_TelNumber, @Starred, @TimeReceived, 
		@TelNumber, @Support_TelNumber);
		
INSERT INTO Messages ([From], [To], [Text], TimeReceived, [Read], ConversationId)
		VALUES (@Support_TelNumber, @TelNumber, @WelcomeMessage,
		@TimeReceived, @Read, @ConversationId);

UPDATE WorkingPoints SET SupportConversation_ConvId = @ConversationId WHERE TelNumber = @TelNumber;

COMMIT TRAN