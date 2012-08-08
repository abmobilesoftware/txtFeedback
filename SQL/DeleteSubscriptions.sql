USE txtfeedback_dev
DECLARE @SubscriptionType nvarchar(50) = 'Premium';

DELETE FROM Subscriptions WHERE Type = @SubscriptionType; 

