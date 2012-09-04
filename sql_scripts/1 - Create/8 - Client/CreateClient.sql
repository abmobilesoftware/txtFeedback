USE txtfeedback_production

--Important fields
DECLARE @Client_TelNumber nvarchar(50) = '4915706107678';
DECLARE @Client_DisplayName nvarchar(50) = 'TxtSupport';

--Less used
DECLARE @Client_Description nvarchar(160) = @Client_DisplayName + ' client';
DECLARE @Client_isSupportClient bit = 1;

-- Add client
INSERT INTO Clients(TelNumber, DisplayName,
		Description, isSupportClient)
		VALUES (@Client_TelNumber, @Client_DisplayName, 
		@Client_Description, @Client_isSupportClient);		
