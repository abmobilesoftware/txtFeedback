USE txtfeedback_dev
DECLARE @SubscriptionType nvarchar(50) = 'Premium';
DECLARE @NrOfWorkingPoints int = 1;
DECLARE @SmsPerWorkingPoint int = 50;
DECLARE @NrOfUsers int = 3;
DECLARE @NrOfHoursOfTraining nvarchar(MAX) = '0';

INSERT INTO txtfeedback_dev.dbo.Subscriptions(Type, NrOfWorkingPoints, 
		SmsPerWorkingPoint, NrOfUsers, NrOfHoursOfTraining) 
		VALUES (@SubscriptionType, @NrOfWorkingPoints, @SmsPerWorkingPoint, 
		@NrOfUsers, @NrOfHoursOfTraining );

