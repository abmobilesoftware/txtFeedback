SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_dev

-- Important fields
DECLARE @CompanyName nvarchar(50) = 'Lidl78974Test';
DECLARE @CompanyDescription nvarchar(160) = 'Support for Txtfeedback';
DECLARE @CompanyAddress nvarchar(160) = 'Dorobantilor, 89';
DECLARE @Subscription_Type nvarchar(50) = 'Special';
DECLARE @NrOfTrainingHoursDelivered int = 0;

DECLARE @TagDescription nvarchar(200) = 'default tag';
-- English version
DECLARE @TagPositive nvarchar(50) = 'Positive feedback';
DECLARE @TagNegative nvarchar(50) = 'Negative feedback';
-- German version
--DECLARE @TagPositive nvarchar(50) = 'Positives feedback';
--DECLARE @TagNegative nvarchar(50) = 'Negatives feedback';

DECLARE @NegativeTagType NVARCHAR(50) = 'negativeFeedback';
DECLARE @PositiveTagType NVARCHAR(50) = 'positiveFeedback';

INSERT INTO dbo.Companies(Name, [Description], 
		Subscription_Type, [Address], NrOfTrainingHoursDelivered) 
		VALUES (@CompanyName, @CompanyDescription, @Subscription_Type, 
		@CompanyAddress, @NrOfTrainingHoursDelivered );
		
INSERT INTO dbo.Tags(Name, [Description], CompanyName) VALUES (@TagPositive, @TagDescription, @CompanyName);
INSERT INTO dbo.Tags(Name, [Description], CompanyName) VALUES (@TagNegative, @TagDescription, @CompanyName);

INSERT INTO dbo.TagTagTypes
        ( IsDefault ,
          TagName ,
          TagCompanyName ,
          TagTypeType
        )
VALUES  ( 1 , -- IsDefault - bit
          @TagPositive , -- TagName - nvarchar(50)
          @CompanyName , -- TagCompanyName - nvarchar(50)
          @PositiveTagType  -- TagTypeType - nvarchar(50)
        )

INSERT INTO dbo.TagTagTypes
        ( IsDefault ,
          TagName ,
          TagCompanyName ,
          TagTypeType
        )
VALUES  ( 1 , -- IsDefault - bit
          @TagNegative , -- TagName - nvarchar(50)
          @CompanyName , -- TagCompanyName - nvarchar(50)
          @NegativeTagType -- TagTypeType - nvarchar(50)
        )        

COMMIT TRAN