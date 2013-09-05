SET XACT_ABORT ON
BEGIN TRAN

-- Important fields
DECLARE @Client_TelNumber nvarchar(50) = '409917000123';
DECLARE @WP_Name nvarchar(40) = 'Sales demo 2';
DECLARE @Client_Description nvarchar(160) = 'Sales demo 2';
DECLARE @Support_TelNumber nvarchar(50) = '2220000100';
DECLARE @WP_ShortID nvarchar(10) = 'store6min';
DECLARE @WP_XmppSuffix nvarchar(50) = '@compdev.txtfeedback.net';

DECLARE @WP_Support_TelNumber nvarchar(50) = '2220000100';
DECLARE @WP_Support_XmppSuffix nvarchar(50) = '@compdev.txtfeedback.net';
DECLARE @WP_Support_ShortID nvarchar(10) = 'supportro';

-- Less used
DECLARE @WP_Provider nvarchar(50) = 'vivacom';
DECLARE @WP_SmsSent int = 0;
DECLARE @WP_MaxNrOfSmsToSend int = 200;
DECLARE @Description_Additional_Text nvarchar(120) = ' WP';
DECLARE @WP_Description nvarchar(160) = @WP_Name + @Description_Additional_Text;
-- Welcome message
DECLARE @WP_WelcomeMessage nvarchar(160) = 'Bine aþi venit la TxtFeedback. Cu ce vã putem fi de folos?';
DECLARE @WP_BusyMessage nvarchar(160) = 'Vã mulþumim de feedback. Vã rugãm îngãduiþi pânã la 2 minute pentru un rãspuns';
DECLARE @WP_OutsideOfficeHoursMessage nvarchar(160) = 'Outside of office hours';
DECLARE @WP_ConvIdSupportToWP nvarchar(50);
DECLARE @WP_ConvIdWPToSupport nvarchar(50);

-- Languages for working points
DECLARE @WP_Language_RO nvarchar(10) = 'ro-RO';
DECLARE @WP_Language_US nvarchar(10) = 'en-US';
DECLARE @WP_Language_DE nvarchar(10) = 'de-DE';

DECLARE @TimeReceived datetime = current_timestamp;
DECLARE @Read bit = 0;
--support conversations 
DECLARE @WP_TimeReceived datetime = GETUTCDATE();
DECLARE @ConvIdSupportToWP nvarchar(50) = '';
DECLARE @ConvIdWPToSupport nvarchar(50) = '' ;
DECLARE @Starred bit = 1;
DECLARE @WP_Read bit = 0;
DECLARE @WP_Starred bit = 0;
DECLARE @Client_Support_DisplayName nvarchar(50) = 'TxtSupport';
DECLARE @WP_Support_Description nvarchar(120)= 'Support pentru Romania ';

SET @ConvIdSupportToWP = @Support_TelNumber + '-' + @Client_TelNumber;
SET @ConvIdWPToSupport = @Client_TelNumber + '-' + @Support_TelNumber;
DECLARE @WP_Support_WelcomeMessage nvarchar(160) = 'Bine aþi venit la TxtFeedback. Folosiþi aceastã conversaþie pentru a clarifica orice/toate întrebãrile';
--Client
DECLARE @Client_isSupportClient bit = 0; 
DECLARE @Support_isSupportClient bit = 1;
DECLARE @isSmsBased_IM bit = 0;
DECLARE @isSmsBased_SMS bit = 1;

-- Add working point
IF (SELECT COUNT(*) FROM [dbo].[WorkingPoints] WHERE [dbo].[WorkingPoints].[TelNumber] = @Client_TelNumber) = 0 
INSERT INTO WorkingPoints(TelNumber, Description, Name, 
		Provider, SentSms, MaxNrOfSmsToSend, ShortID, XMPPsuffix, WelcomeMessage,
		BusyMessage, OutsideOfficeHoursMessage, [Language]) 
		VALUES (@Client_TelNumber, @WP_Description, @WP_Name, 
		@WP_Provider, @WP_SmsSent, @WP_MaxNrOfSmsToSend, @WP_ShortID, 
		@WP_XmppSuffix, @WP_WelcomeMessage, @WP_BusyMessage,
		@WP_OutsideOfficeHoursMessage, @WP_Language_RO);

COMMIT TRAN