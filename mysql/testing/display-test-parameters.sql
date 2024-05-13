/*
	create a list of email accounts that are used for specific test cases
    
*/
SET @testName = 'Register Account.Create account';

SELECT * 
FROM TESTPARAMETER
WHERE `Test` = @testName
ORDER BY `Test`, IterationId, OrderId;