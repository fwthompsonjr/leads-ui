/*
	append test parameter
*/
SET @testName = 'Profile.Change Address - should complete successfully';
SET @iterationId = 1;
SET @accountEmail = (SELECT `Name` FROM testing.ACCOUNTS WHERE `Name` LIKE '%live%' LIMIT 1);
SET @accountUserName = (SELECT UserName FROM testing.USERSCLONE WHERE Email = @accountEmail LIMIT 1);
SET @pword = (SELECT `InternalId` FROM testing.ACCOUNTS WHERE `Name` LIKE '%live%' LIMIT 1);
DROP TEMPORARY TABLE IF EXISTS tmp_user_name;
DROP TEMPORARY TABLE IF EXISTS tmp_field_name;
CREATE TEMPORARY TABLE tmp_field_name
(
	Id INT NOT NULL AUTO_INCREMENT,
	FieldName varchar(255),
    FieldValue varchar(255),
    PRIMARY KEY ( Id )
);

SET @streetName1 = (SELECT FieldValue FROM testing.RANDOMADDRESS WHERE `Name` = 'STREET' ORDER BY RAND() LIMIT 1);
SET @streetName2 = (SELECT FieldValue FROM testing.RANDOMADDRESS WHERE `Name` = 'STREET' ORDER BY RAND() LIMIT 1);

SET @locationName1 = (SELECT FieldValue FROM testing.RANDOMADDRESS WHERE `Name` = 'LOCATION' ORDER BY RAND() LIMIT 1);
SET @locationName2 = (SELECT FieldValue FROM testing.RANDOMADDRESS WHERE `Name` = 'LOCATION' ORDER BY RAND() LIMIT 1);

SET @hasMailAddress = 1;
SET @hasBillAddress = 1;

SET @mailAddress = CASE WHEN @hasMailAddress = 1 THEN CONCAT( @streetName1, ', ', @locationName1 ) ELSE '' END;
SET @billAddress = CASE WHEN @hasBillAddress = 1 THEN CONCAT( @streetName2, ', ', @locationName2 ) ELSE '' END;

INSERT tmp_field_name ( FieldName, FieldValue )
VALUES
	( 'UserName', @accountUserName ),
    ( 'Password', @pword ),
    ( 'Mailing Address', @mailAddress ),
    ( 'Billing Address', @billAddress );
    
INSERT testing.TESTPARAMETER ( Test, IterationId, OrderId, FieldName, FieldValue )
SELECT 
	@testName `Test`,
    @iterationId IterationId,
    fn.Id OrderId,
    fn.FieldName,
    fn.FieldValue
  FROM tmp_field_name fn
  LEFT
  JOIN testing.TESTPARAMETER p 
    ON @testName = p.`Test` 
    AND @iterationId = p.IterationId
    AND fn.Id = p.OrderId
  WHERE	p.`Test` IS NULL
  ORDER BY fn.Id;
  
