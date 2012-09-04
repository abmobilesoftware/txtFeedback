USE txtfeedback_production

-- Important fields
DECLARE @SubscriptionType nvarchar(50) = 'Free';
DECLARE @NrOfWorkingPoints int = 1;
DECLARE @SmsPerWorkingPoint int = 50;
DECLARE @NrOfUsers int = 3;
DECLARE @NrOfHoursOfTraining nvarchar(MAX) = '0';

INSERT INTO dbo.Subscriptions(Type, NrOfWorkingPoints, 
		SmsPerWorkingPoint, NrOfUsers, NrOfHoursOfTraining) 
		VALUES (@SubscriptionType, @NrOfWorkingPoints, @SmsPerWorkingPoint, 
		@NrOfUsers, @NrOfHoursOfTraining );

