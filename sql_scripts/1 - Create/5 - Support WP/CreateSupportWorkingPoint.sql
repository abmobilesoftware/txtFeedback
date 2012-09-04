SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production;

-- Important fields
DECLARE @TelNumber nvarchar(50) = '16783694507';
DECLARE @Name nvarchar(40) = 'US support';

-- Less used
DECLARE @Description_Additional_Text nvarchar(120) = ' WP';
DECLARE @Description nvarchar(160) = @Name + @Description_Additional_Text;
DECLARE @Provider nvarchar(50) = 'nexmo';
DECLARE @SmsSent int = 0;
DECLARE @MaxNrOfSmsToSend int = 200;
DECLARE @Client_DisplayName nvarchar(50) = 'TxtSupport';
DECLARE @Client_Description nvarchar(160) = @Client_DisplayName + ' client';
DECLARE @Client_isSupportClient bit = 1;

-- Create WP
INSERT INTO WorkingPoints(TelNumber, [Description], Name, 
		Provider, SentSms, MaxNrOfSmsToSend) 
		VALUES (@TelNumber, @Description, @Name, 
		@Provider, @SmsSent, @MaxNrOfSmsToSend);

INSERT INTO Clients(TelNumber, DisplayName,
		Description, isSupportClient)
		VALUES (@TelNumber, @Client_DisplayName, 
		@Client_Description, @Client_isSupportClient);		

COMMIT TRAN