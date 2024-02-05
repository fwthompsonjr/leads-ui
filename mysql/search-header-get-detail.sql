SET @usr = 'cf35094a-ad64-41dd-9f2d-32cbc942aaed';
SELECT 
Id, StartDate, EndDate, ExpectedRows, CreateDate, SearchProgress,
CASE WHEN LineJs IS NULL THEN 'N/A'
	ELSE UPPER( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.state') ) ) END StateCode,
CASE WHEN LineJs IS NULL THEN 'N/A'
	ELSE UPPER( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.county.name') ) ) END CountyName
FROM (
SELECT 
S.Id,
S.StartDate, 
S.EndDate, 
S.ExpectedRows, 
S.CreateDate,
    CASE 
		WHEN EndDate is null AND ExpectedRows is NULL THEN "1 - Submitted"
        WHEN EndDate is null AND ExpectedRows is not NULL THEN "2 - Processing"
        WHEN EndDate is not null AND ExpectedRows is not NULL AND ExpectedRows > 0 THEN "3 - Completed"
        ELSE "4 - Error" END SearchProgress,
  CAST( CONVERT(`Line` USING UTF8MB4)  AS JSON ) `LineJs`
  FROM SEARCH S
  LEFT JOIN SEARCHREQUEST R ON S.Id = R.SearchId AND 1 = LineNbr
  WHERE UserId = @usr
) a
ORDER BY a.CreateDate DESC;