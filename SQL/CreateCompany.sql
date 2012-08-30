DECLARE @TransactionName varchar(30) = 'CreateCompany';

BEGIN TRANSACTION @TransactionName
USE txtfeedback_dev
DECLARE @CompanyName nvarchar(50) = 'AbMobileGlobal';
DECLARE @CompanyDescription nvarchar(MAX) = 'Music company';
DECLARE @CompanyAddress nvarchar(MAX) = 'Beatles street';
DECLARE @Subscription_Type nvarchar(50) = 'Free';
DECLARE @NrOfTrainingHoursDelivered int = 0;

IF (SELECT COUNT(*) FROM txtfeedback_dev.dbo.Subscriptions WHERE TYPE = @Subscription_Type) = 0
BEGIN
	PRINT 'Subscription does not exist. Create a subscription and then rerun this script';
END
ELSE
BEGIN
	INSERT INTO txtfeedback_dev.dbo.Companies(Name, Description, 
			Subscription_Type, Address, NrOfTrainingHoursDelivered) 
			VALUES (@CompanyName, @CompanyDescription, @Subscription_Type, 
			@CompanyAddress, @NrOfTrainingHoursDelivered );
END

COMMIT TRANSACTION @TransactionName;