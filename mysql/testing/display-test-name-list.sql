-- display tests defined in suite
SET @testSuite = 'Permissions';
SET @locator = CONCAT( @testSuite, '%' );
SELECT nn.*, p.`Test`
FROM (
	SELECT 
		n.Id, n.`Name`
	FROM (
		SELECT nn.*, CONCAT( nn.`Name`, '%' ) Findex
		FROM testing.TESTNAMES nn
		WHERE nn.`Name` = @testSuite) n
	) nn
CROSS
JOIN ( SELECT `Test` 
		  FROM testing.TESTPARAMETER 
          WHERE `Test` 
          LIKE @locator
         GROUP BY `Test` ) p 
ORDER BY p.`Test`