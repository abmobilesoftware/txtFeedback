USE txtfeedback_production

-- Important fields
-- SubscriptionDetails
DECLARE @S_PrimaryContact_Name nvarchar(50) = 'primary'; 
DECLARE @S_PrimaryContact_Surname nvarchar(50) = 'primarySurname';
DECLARE @S_PrimaryContact_Email nvarchar(50) = 'primary@primary.com';
DECLARE @S_SecondaryContact_Name nvarchar(50) = 'secondary'; 
DECLARE @S_SecondaryContact_Surname nvarchar(50) = 'secondarySurname';
DECLARE @S_SecondaryContact_Email nvarchar(50) = 'secondary@secondary.com';

-- Less important
-- Subscription details 
DECLARE @S_SpendingLimit decimal = 10.0;
DECLARE @S_WarningLimit decimal = @S_SpendingLimit * 9 /10;
DECLARE @S_SubscriptionSMS int = 20;

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

