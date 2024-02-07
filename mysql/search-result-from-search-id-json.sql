-- note: the filing-date is not visible in final json
SET @user_id = 'cf35094a-ad64-41dd-9f2d-32cbc942aaed';
SET @search_id = (
SELECT Id
  FROM SEARCH
  WHERE UserId = @user_id
  ORDER BY CreateDate DESC
  LIMIT 1);
  
SET @search_staging_id = (
SELECT Id
  FROM SEARCHSTAGING
  WHERE SearchId = @search_id
    AND StagingType = 'data-output-person-addres'
    ORDER BY LineNbr DESC
    LIMIT 1);

SET @txt = (
SELECT
CASE WHEN IsBinary = 0 THEN CONVERT(`Line` USING UTF8MB4) ELSE NULL END LineText
FROM defaultdb.SEARCHSTAGING
WHERE ID = @search_staging_id);
SET @jstable = CAST( @txt as JSON);

SELECT @search_id SearchId,
	get_ddi.*
FROM
JSON_TABLE(@jstable, '$[*]' COLUMNS (
                `Name` VARCHAR(150)  PATH '$.Name',
                `Zip` VARCHAR(150)  PATH '$.Zip',
                `Address1` VARCHAR(150)  PATH '$.Address1',
                `Address2` VARCHAR(150)  PATH '$.Address2',
                `Address3` VARCHAR(150)  PATH '$.Address3',
                `CaseNumber` VARCHAR(150)  PATH '$.CaseNumber',
                `DateFiled` VARCHAR(150)  PATH '$.DateFiled', -- this field is blank
                `Court` VARCHAR(150)  PATH '$.Court', -- this is potentially incorrect, always same value
                `CaseType` VARCHAR(150)  PATH '$.CaseType', -- case type is actually showing the date-filed
                `CaseStyle` VARCHAR(150)  PATH '$.CaseStyle', -- this item is always the first case in dataset
                `FirstName` VARCHAR(150)  PATH '$.FirstName',
                `LastName` VARCHAR(150)  PATH '$.LastName',
                `Plantiff` VARCHAR(150)  PATH '$.Plantiff',
                `Status` VARCHAR(150)  PATH '$.Status' -- this field is blank
                )
     ) get_ddi;