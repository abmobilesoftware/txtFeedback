USE txtfeedback_dev

DECLARE @TelNumber nvarchar(50) = '12898437378';
DECLARE @DisplayName nvarchar(50) = 'CA Support';
DECLARE @Description nvarchar(160) = 'CA Support Client';
DECLARE @isSupportClient bit = 1;

INSERT INTO Clients(TelNumber, DisplayName,
		Description, isSupportClient)
		VALUES (@TelNumber, @DisplayName, 
		@Description, @isSupportClient);		

