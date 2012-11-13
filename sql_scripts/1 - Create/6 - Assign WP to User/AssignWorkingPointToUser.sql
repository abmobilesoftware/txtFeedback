USE txtfeedback_production;

--Important fields
DECLARE @UserId uniqueidentifier = 'a29f8c80-433e-4483-8501-580f3df22d19';
DECLARE @WorkingPointTelNumber nvarchar(50) = '0000000001';

INSERT INTO UsersForWorkingPoints (Users_UserId, WorkingPoints_TelNumber) 
			VALUES (@UserId, @WorkingPointTelNumber);	