/*
	as a developer,
    i want to set the final record count in the search table
    when a seach has been completed
    row_number() over(order by hire_date)
*/



SET SQL_SAFE_UPDATES = 0;
CALL USP_SEARCH_SET_FINAL_ROW_COUNT();

SET @user_index = (SELECT Id
  FROM USERS
  WHERE UserName = 'test.account');
  
DROP TEMPORARY TABLE IF EXISTS tmp_user_searches;
CREATE TEMPORARY TABLE tmp_user_searches
	SELECT 
		row_number() over(order by CreateDate) Nbr,
        Id, 
        ExpectedRows,
        0 ItemCount
    FROM SEARCH
    WHERE UserId = @user_index
    ORDER BY CreateDate;
SET @nbr = 24;    
SET @search_index = (SELECT Id
  FROM tmp_user_searches
  WHERE Nbr = @nbr);

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

SET @item_count = (
SELECT COUNT( 1 )
FROM
JSON_TABLE(@jstable, '$[*]' COLUMNS (
                `Zip` VARCHAR(150)  PATH '$.Zip',
                `FirstName` VARCHAR(150)  PATH '$.FirstName',
                `LastName` VARCHAR(150)  PATH '$.LastName'
                )
     ) get_ddi);
     
UPDATE tmp_user_searches
  SET ItemCount = @item_count
  WHERE Nbr = @nbr
    AND Id = @search_index;
  
SELECT *
  FROM tmp_user_searches;
