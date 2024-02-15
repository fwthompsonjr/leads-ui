-- 'e7b3d118-cc10-11ee-be26-0af7a01f52e9' Collin Example
-- '99f9d191-cc11-11ee-be26-0af7a01f52e9' Tarrant Example
-- 'debb1c55-c5d0-11ee-be26-0af7a01f52e9' Denton Example
SET @search_index = 'debb1c55-c5d0-11ee-be26-0af7a01f52e9';

SET @ss_parm = (
SELECT CAST( CONVERT(`Line` USING UTF8MB4) AS JSON ) Line
  FROM SEARCHREQUEST
  WHERE SearchId = @search_index
  ORDER BY CreateDate DESC
  LIMIT 1);

  
SET @county_name = (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.county.name" ) ));


SET @search_staging_id = (
SELECT Id
  FROM SEARCHSTAGING
  WHERE SearchId = @search_index
    AND StagingType LIKE 'data-output-person-addres%'
    ORDER BY LineNbr DESC
    LIMIT 1);

SET @txt = (
SELECT
CASE WHEN IsBinary = 0 THEN CONVERT(`Line` USING UTF8MB4) ELSE NULL END LineText
FROM defaultdb.SEARCHSTAGING
WHERE ID = @search_staging_id);
SET @jstable = CAST( @txt as JSON);

DROP TEMPORARY TABLE IF EXISTS tb_tmp_search_address_list;
CREATE TEMPORARY TABLE tb_tmp_search_address_list
SELECT SearchId, 
`Name`, 
Zip, 
Address1, 
Address2, 
Address3, 
CaseNumber, 
DateFiled, 
Court, 
CaseType, 
CaseStyle, 
FirstName, 
LastName, 
Plantiff, 
`Status`
  FROM (

SELECT @search_index SearchId,
	CASE 
    WHEN STR_TO_DATE( get_ddi.DateFiled, "%m/%d/%Y") IS NULL THEN '1900-01-01'
    ELSE STR_TO_DATE( get_ddi.DateFiled, "%m/%d/%Y") END
    DtFiled,
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
     ) get_ddi
     ) a
     ORDER BY DtFiled, CaseNumber;
     
SELECT 
	SearchId, `Name`, Zip, Address1, Address2, Address3, 
    CaseNumber, DateFiled, Court, CaseType, CaseStyle, 
    FirstName, LastName, Plantiff, 
    -- `Status`, -- this field is not populated correctly from searches excluded from data.
    County, 
    CASE WHEN CourtAddress IS NULL THEN '' ELSE CourtAddress END CourtAddress
FROM
(     
SELECT 
	aa.*,
	UPPER( @county_name ) County,
    (SELECT ca.Address
	  FROM aspnettds.mds_court_address ca
	  WHERE 
	( ca.`Name` = aa.Court  OR ca.FullName = aa.Court ) LIMIT 1) CourtAddress
    
  FROM tb_tmp_search_address_list aa
  ) q;