/*
	create a list of email accounts that are used for specific test cases
    
*/
SET SQL_SAFE_UPDATES = 0;
SET @testName = 'Account%';


SELECT * 
FROM testing.TESTPARAMETER
WHERE `Test` LIKE @testName
ORDER BY `Test`, IterationId, OrderId;