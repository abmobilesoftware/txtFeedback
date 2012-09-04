USE txtfeedback_production;

--Important fields
DECLARE @UserId uniqueidentifier = 'd6e62ef1-e6eb-4110-b828-c18a75dbe51e';
DECLARE @WorkingPointTelNumber nvarchar(50) = '442033222167';

INSERT INTO UsersForWorkingPoints (Users_UserId, WorkingPoints_TelNumber) 
			VALUES (@UserId, @WorkingPointTelNumber);	