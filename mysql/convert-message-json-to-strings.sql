set @js = '{
	"From": {
		"DisplayName": "",
		"User": "admin",
		"Host": "somewhere.org",
		"Address": "admin@somewhere.org"
	},
	"To": [
		{
			"DisplayName": "",
			"User": "person",
			"Host": "places.com",
			"Address": "person@places.com"
		}
	],
	"Cc": [],
	"Subject": "Test",
	"Body": "Test"
}';
SET @subjectLine = (SELECT JSON_UNQUOTE(JSON_EXTRACT(@js, '$.Subject')));
SET @bodyHtml = (SELECT JSON_UNQUOTE(JSON_EXTRACT(@js, '$.Body')));
SET @fromAddress = (
SELECT CONCAT( jfrom.Email, 
	CASE WHEN LENGTH(jfrom.DisplayName) = 0 THEN ''
    ELSE CONCAT( ' <', jfrom.DisplayName, '>') END ) FromAddress
  FROM json_table( @js, "$.From"
	COLUMNS( 
		DisplayName VARCHAR(255) PATH '$.DisplayName',
        Email VARCHAR(255) PATH '$.Address' ) ) jfrom);
        
SET @toAddresses = (
SELECT GROUP_CONCAT(ToAddress SEPARATOR ', ') ToAddresses
FROM (
SELECT CONCAT( jto.Email, 
	CASE WHEN LENGTH(jto.DisplayName) = 0 THEN ''
    ELSE CONCAT( ' <', jto.DisplayName, '>') END ) ToAddress
  FROM json_table( @js, "$.To[*]"
	COLUMNS( 
		DisplayName VARCHAR(255) PATH '$.DisplayName',
        Email VARCHAR(255) PATH '$.Address' ) ) jto ) a);
        
SET @copyAddresses = (
SELECT GROUP_CONCAT(ToAddress SEPARATOR ', ') ToAddresses
FROM (
SELECT CONCAT( jto.Email, 
	CASE WHEN LENGTH(jto.DisplayName) = 0 THEN ''
    ELSE CONCAT( ' <', jto.DisplayName, '>') END ) ToAddress
  FROM json_table( @js, "$.Cc[*]"
	COLUMNS( 
		DisplayName VARCHAR(255) PATH '$.DisplayName',
        Email VARCHAR(255) PATH '$.Address' ) ) jto ) a);
        
SELECT 
	@subjectLine `SubjectLine`,
	@fromAddress `FromAddress`,
    @toAddresses `ToAddresses`,
    @copyAddresses `CopyAddresses`,
    @bodyHtml `HtmlBody`;