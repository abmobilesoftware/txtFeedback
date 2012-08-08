SET XACT_ABORT ON
BEGIN TRAN

USE txtfeedback_dev
DECLARE @TelNumber nvarchar(50) = '441157070699';
DECLARE @SupportConversationId nvarchar(50);

SET @SupportConversationId = (SELECT SupportConversation_ConvId From WorkingPoints WHERE TelNumber = @TelNumber);

UPDATE WorkingPoints SET SupportConversation_ConvId = NULL WHERE TelNumber = @TelNumber;
DELETE FROM Messages WHERE ConversationId = @SupportConversationId;
DELETE FROM Conversations WHERE ConvId = @SupportConversationId;
DELETE FROM WorkingPoints WHERE TelNumber = @TelNumber;

COMMIT TRAN;