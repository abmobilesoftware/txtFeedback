USE txtfeedback_production  -- choose db

DECLARE @SubscriptionType nvarchar(50) = 'Free';
DELETE FROM Subscriptions WHERE [Type] = @SubscriptionType; 

