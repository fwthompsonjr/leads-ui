/*
	create a list of email accounts that are used for specific test cases
    Profile.Change Address - should complete successfully
	Profile.Change Email - should complete successfully
	Profile.Change Name - should complete successfully
	Profile.Change Phone - should complete successfully
*/
SET SQL_SAFE_UPDATES = 0;
SET @testName = 'Profile.Change%';


SELECT * 
FROM testing.TESTPARAMETER
WHERE `Test` LIKE @testName
ORDER BY `Test`, IterationId, OrderId;