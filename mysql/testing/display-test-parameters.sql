/*
	create a list of email accounts that are used for specific test cases
    
*/
SET @testName = 'Profile%';

SELECT * 
FROM testing.TESTPARAMETER
WHERE `Test` LIKE @testName
ORDER BY `Test`, IterationId, OrderId;