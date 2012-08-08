USE txtfeedback_dev

DECLARE @CompanyName nvarchar(50) = 'AbMobileApps';
DELETE FROM Companies WHERE Name = @CompanyName;
