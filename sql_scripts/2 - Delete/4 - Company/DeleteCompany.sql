USE txtfeedback_production -- choose db

DECLARE @CompanyName nvarchar(50) = 'Cluj Arena';
DELETE FROM Companies WHERE Name = @CompanyName;
