USE txtfeedback_dev

DECLARE @TelNumber nvarchar(50) = '441157070678';
DELETE FROM Clients WHERE TelNumber = @TelNumber;

