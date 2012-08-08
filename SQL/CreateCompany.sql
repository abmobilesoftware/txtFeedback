SET XACT_ABORT ON
BEGIN TRAN

USE txtfeedback_dev
DECLARE @CompanyName nvarchar(50) = 'AbMobileApps';
DECLARE @CompanyDescription nvarchar(MAX) = 'Music company';
DECLARE @CompanyAddress nvarchar(MAX) = 'Beatles street';
DECLARE @Subscription_Type nvarchar(50) = 'Premium';
DECLARE @NrOfTrainingHoursDelivered int = 0;

INSERT INTO txtfeedback_dev.dbo.Companies(Name, Description, 
		Subscription_Type, Address, NrOfTrainingHoursDelivered) 
		VALUES (@CompanyName, @CompanyDescription, @Subscription_Type, 
		@CompanyAddress, @NrOfTrainingHoursDelivered );

COMMIT TRAN