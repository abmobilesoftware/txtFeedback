SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_production -- choose db

-- Important fields
DECLARE @TelNumber nvarchar(50) = '12152401393';

-- Internal use
DECLARE @SupportConversationId nvarchar(50);
SET @SupportConversationId = (SELECT SupportConversation From WorkingPoints WHERE TelNumber = @TelNumber);

UPDATE WorkingPoints SET SupportConversation = NULL WHERE TelNumber = @TelNumber;
DELETE FROM Messages WHERE ConversationId = (SELECT ConvId FROM Conversations WHERE WorkingPoint_TelNumber = @TelNumber);
DELETE FROM Conversations WHERE WorkingPoint_TelNumber = @TelNumber
DELETE FROM WorkingPoints WHERE TelNumber = @TelNumber;

COMMIT TRAN;