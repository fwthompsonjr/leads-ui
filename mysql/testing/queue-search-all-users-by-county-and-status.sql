SET @countyFilter = null; -- 'HARRIS';
SET @statusFilter = null; -- '10 - Error';

SELECT
		v.*,
        s.StartDate RequestStartDt,
        s.EndDate RequestEndDt
FROM VW_SEARCH_STATUS_ALL v
JOIN SEARCH s
  ON s.Id = v.Id
WHERE 1 = 1
AND v.CountyName 		= CASE WHEN @countyFilter IS NULL THEN v.CountyName ELSE @countyFilter END
AND v.SearchProgress 	= CASE WHEN @statusFilter IS NULL THEN v.SearchProgress ELSE @statusFilter END;


/*
UPDATE 	SEARCH s
  SET	s.EndDate = NULL,
		s.ExpectedRows = null
WHERE	s.Id = '7b10201a-40b2-11ef-99ce-0af7a01f52e9';
*/