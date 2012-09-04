USE txtfeedback_production -- choose db

DECLARE @TelNumber nvarchar(50) = '12152401393';
DELETE FROM Clients WHERE TelNumber = @TelNumber;

