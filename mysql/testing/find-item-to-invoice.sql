SET @usrid = (SELECT Id FROM USERS u WHERE `UserName` = 'test.account' LIMIT 1);
SET @indx = 'b5b9d3fa-6bf4-11ef-99ce-0af7a01f52e9';
-- CALL USP_QUERY_USER_SEARCH( @usrid );
DROP TEMPORARY TABLE IF EXISTS tmp_my_search;
CREATE TEMPORARY TABLE tmp_my_search
SELECT 
Id, UserId, ExpectedRows, CreateDate, SearchProgress, 
StateCode, CountyName, EndDate, StartDate
FROM VW_SEARCH_STATUS_ALL b
WHERE b.CountyName != 'N/A'
AND b.SearchProgress = '3 - Completed'
AND b.UserId = @usrid
ORDER BY b.CreateDate DESC;

SELECT *
  FROM tmp_my_search
  WHERE Id = @indx;