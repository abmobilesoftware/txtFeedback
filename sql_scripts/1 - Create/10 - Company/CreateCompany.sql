SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_dev

-- Important fields
-- Company
DECLARE @U_CompanyName nvarchar(50) = 'ANameThatDoesNotExist'; -- details in less used section
DECLARE @C_ContactName nvarchar(50) = 'companyContactName';
DECLARE @C_ContactSurname nvarchar(50) = 'companyContactSurname';
DECLARE @C_ContactEmail nvarchar(50) = 'company@company.com';
DECLARE @C_VATID nvarchar(50) = 'thisIsADemoID';
DECLARE @C_RegistrationNumber nvarchar(50) = NULL;
-- SubscriptionDetails
DECLARE @S_PrimaryContact_Name nvarchar(50) = 'primary'; 
DECLARE @S_PrimaryContact_Surname nvarchar(50) = 'primarySurname';
DECLARE @S_PrimaryContact_Email nvarchar(50) = 'primary@primary.com';
DECLARE @S_SecondaryContact_Name nvarchar(50) = 'secondary'; 
DECLARE @S_SecondaryContact_Surname nvarchar(50) = 'secondarySurname';
DECLARE @S_SecondaryContact_Email nvarchar(50) = 'secondary@secondary.com';

-- Less important
-- Company
DECLARE @C_Description nvarchar(max) = 'new company';
DECLARE @C_Address nvarchar(max) = 'company address';
DECLARE @C_City nvarchar(50) = 'company city';

-- Subscription details 
DECLARE @S_SpendingLimit decimal = 10.0;
DECLARE @S_WarningLimit decimal = @S_SpendingLimit * 9 /10;
DECLARE @S_SubscriptionSMS int = 20;


DECLARE @TagDescription nvarchar(200) = 'default tag';
-- English version
DECLARE @TagPositive nvarchar(50) = 'Positive feedback';
DECLARE @TagNegative nvarchar(50) = 'Negative feedback';
-- German version
--DECLARE @TagPositive nvarchar(50) = 'Positives feedback';
--DECLARE @TagNegative nvarchar(50) = 'Negatives feedback';

DECLARE @NegativeTagType NVARCHAR(50) = 'negativeFeedback';
DECLARE @PositiveTagType NVARCHAR(50) = 'positiveFeedback';

-- Check the company
IF (SELECT COUNT(*) FROM [dbo].[Companies] WHERE [dbo].[Companies].[Name] = @U_CompanyName) = 0 
BEGIN
--we don't have the company so
--Check for primary contact (we don't reuse the contact, but create a new one
DECLARE @id_primaryContact int = 0;
INSERT INTO Contacts ([Name],[Surname],[Email],[Landline],[Mobile])
VALUES (@S_PrimaryContact_Name,@S_PrimaryContact_Surname,@S_PrimaryContact_Email,NULL,NULL) SET @id_primaryContact = SCOPE_IDENTITY();
--Check for secondary contact
DECLARE @id_secondaryContact int = 0;
INSERT INTO Contacts ([Name],[Surname],[Email],[Landline],[Mobile])
VALUES (@S_SecondaryContact_Name,@S_SecondaryContact_Surname,@S_SecondaryContact_Email,NULL,NULL) SET @id_secondaryContact = SCOPE_IDENTITY();
--Check for SubscriptionTemplate ID
DECLARE @id_invoiceSubscriptionTemplateID int = NULL;
--Check for ExtraCreditTemplate ID
DECLARE @id_invoiceExtraCreditTemplateID int = NULL;
--Add the subscription details

DECLARE @subscriptionDetailsId int = 0;
INSERT INTO SubscriptionDetails ([BillingDay],[SubscriptionSMS] ,[RemainingSMS] , [SpendingLimit],[WarningLimit],[SpentThisMonth],[DefaultCurrency] ,[PrimaryContact_Id] ,[SecondaryContact_Id] ,[MonthlySubscriptionTemplate_InvoiceDetailsTemplateID] ,[MonthlyExtraSMSCharge_InvoiceDetailsTemplateID] ,
[ExtraAddedCreditThisMonth],[RemainingCreditFromPreviousMonth]) 
VALUES (DATENAME(d, GETDATE()), @S_SubscriptionSMS, @S_SubscriptionSMS, @S_SpendingLimit, @S_WarningLimit, 0, 'EUR',  @id_primaryContact, @id_secondaryContact,@id_invoiceSubscriptionTemplateID,@id_invoiceExtraCreditTemplateID,0,0) SET @subscriptionDetailsId = SCOPE_IDENTITY();

--add the company contact details
DECLARE @id_companyContact int = 0;
INSERT INTO Contacts ([Name],[Surname],[Email],[Landline],[Mobile])
VALUES (@C_ContactName, @C_ContactSurname,@C_ContactEmail,NULL,NULL) SET @id_companyContact = SCOPE_IDENTITY();
--add the company 

INSERT INTO Companies ([Name], [Description], [Address], [City], [VATID], [RegistrationNumber], [SubscriptionDetails_Id], [Contact_Id])
	 VALUES (@U_CompanyName, @C_Description, @C_Address, @C_City, @C_VATID,@C_RegistrationNumber, @subscriptionDetailsId, @id_companyContact);

		
INSERT INTO dbo.Tags(Name, [Description], CompanyName) VALUES (@TagPositive, @TagDescription, @U_CompanyName);
INSERT INTO dbo.Tags(Name, [Description], CompanyName) VALUES (@TagNegative, @TagDescription, @U_CompanyName);

INSERT INTO dbo.TagTagTypes
        ( IsDefault ,
          TagName ,
          TagCompanyName ,
          TagTypeType
        )
VALUES  ( 1 , -- IsDefault - bit
          @TagPositive , -- TagName - nvarchar(50)
          @U_CompanyName , -- TagCompanyName - nvarchar(50)
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
          @U_CompanyName , -- TagCompanyName - nvarchar(50)
          @NegativeTagType -- TagTypeType - nvarchar(50)
        )        
END
COMMIT TRAN