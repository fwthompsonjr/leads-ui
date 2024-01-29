SET @txt = (
SELECT
CASE WHEN IsBinary = 0 THEN CONVERT(`Line` USING UTF8MB4) ELSE NULL END LineText
FROM defaultdb.SEARCHSTAGING
WHERE ID =
'66c15a12-bee0-11ee-be26-0af7a01f52e9');
SET @jstable = CAST( @txt as JSON);

SELECT 
	*
FROM
JSON_TABLE(@jstable, '$[*]' COLUMNS (
                `Name` VARCHAR(150)  PATH '$.Name',
                `Zip` VARCHAR(150)  PATH '$.Zip',
                `Address1` VARCHAR(150)  PATH '$.Address1',
                `Address2` VARCHAR(150)  PATH '$.Address2',
                `Address3` VARCHAR(150)  PATH '$.Address3',
                `CaseNumber` VARCHAR(150)  PATH '$.CaseNumber',
                `DateFiled` VARCHAR(150)  PATH '$.DateFiled',
                `Court` VARCHAR(150)  PATH '$.Court',
                `CaseType` VARCHAR(150)  PATH '$.CaseType',
                `CaseStyle` VARCHAR(150)  PATH '$.CaseStyle',
                `FirstName` VARCHAR(150)  PATH '$.FirstName',
                `LastName` VARCHAR(150)  PATH '$.LastName',
                `Plantiff` VARCHAR(150)  PATH '$.Plantiff')
     ) get_ddi;