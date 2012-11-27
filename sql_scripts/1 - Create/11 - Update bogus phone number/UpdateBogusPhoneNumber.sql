SET XACT_ABORT ON
BEGIN TRAN
USE txtfeedback_dev

DECLARE @OLD_WORKING_POINT_NR nvarchar(50) = '52898437375';
DECLARE @NEW_WORKING_POINT_NR nvarchar(50) = '12898437378';
DECLARE @BOGUS_WORKING_POINT_NR nvarchar(50) = '1111111111';

UPDATE Conversations SET WorkingPoint_TelNumber = @BOGUS_WORKING_POINT_NR WHERE WorkingPoint_TelNumber = @OLD_WORKING_POINT_NR;
UPDATE UsersForWorkingPoints SET WorkingPoints_TelNumber = @BOGUS_WORKING_POINT_NR WHERE WorkingPoints_TelNumber = @OLD_WORKING_POINT_NR;
UPDATE WorkingPoints SET TelNumber = @NEW_WORKING_POINT_NR WHERE TelNumber = @OLD_WORKING_POINT_NR;
UPDATE UsersForWorkingPoints SET WorkingPoints_TelNumber = @NEW_WORKING_POINT_NR WHERE WorkingPoints_TelNumber = @BOGUS_WORKING_POINT_NR;
UPDATE Conversations SET WorkingPoint_TelNumber = @NEW_WORKING_POINT_NR WHERE WorkingPoint_TelNumber = @BOGUS_WORKING_POINT_NR;

COMMIT TRAN