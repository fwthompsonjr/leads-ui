-- display number of tests defined in suite

SELECT 
	n.Id, n.`Name`,
    (	
		SELECT COUNT(1) FROM 
		( SELECT `Test` 
		  FROM testing.TESTPARAMETER 
          WHERE `Test` 
          LIKE n.Findex
         GROUP BY `Test` ) tc
	) TestCount
FROM (
	SELECT nn.*, CONCAT( nn.`Name`, '%' ) Findex
	FROM testing.TESTNAMES nn ) n;