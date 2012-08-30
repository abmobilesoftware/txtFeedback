CREATE FUNCTION dbo.Base64Encoding
(
    @bin varbinary(max)
)
RETURNS NVARCHAR(128)
AS
BEGIN   
    return CAST(N'' AS XML).value('xs:base64Binary(xs:hexBinary(sql:variable("@bin")))', 'NVARCHAR(128)')
END
