SET @countyName = 'TARRANT';

DROP temporary table if exists tmp_filter_county_search;
CREATE temporary table tmp_filter_county_search
SELECT *
FROM
(
	SELECT
		q.Id, 
		q.UserId, 
		q.StartDate, 
		q.EndDate, 
		q.ExpectedRows, 
		q.CreateDate,
		UPPER( JSON_VALUE( q.`Line`, "$.county.name") ) `County`,
		convert( from_unixtime( LEFT( JSON_VALUE( q.`Line`, "$.start"), 10 ) ) , DATE) `BeginDt`,
		convert( from_unixtime( LEFT( JSON_VALUE( q.`Line`, "$.end"), 10 ) ) , DATE) `EndingDt`
	FROM
	(
	SELECT s.*, CONVERT(`Line` USING UTF8MB4) `Line`
	  FROM SEARCH s
	  LEFT 
	  JOIN 	SEARCHREQUEST sr
	  ON	s.Id = sr.SearchId
	  WHERE sr.LineNbr = 1
        AND s.EndDate is not null
        AND s.ExpectedRows is not null
	) q
) qq
WHERE qq.County = @countyName
  -- AND qq.ExpectedRows = 0
ORDER BY qq.CreateDate DESC;

select * from tmp_filter_county_search;

/*
UPDATE 	SEARCH s
JOIN	tmp_filter_county_search t
  ON 	s.Id = t.Id
  SET	s.EndDate = NULL,
		s.ExpectedRows = null
WHERE	s.Id = '84460a7a-2e8e-11ef-99ce-0af7a01f52e9';
*/