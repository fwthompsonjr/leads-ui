/*
	append test parameter
*/
SET @testName = 'Register Account.Create account will validate User Name';
SET @iterationId = 2;
SET @accountEmail = (SELECT `Name` FROM ACCOUNTS WHERE `Name` LIKE '%testing%' LIMIT 1);
SET @accountUserName = 'Abcd123';
SET @pword = (SELECT `InternalId` FROM ACCOUNTS WHERE `Name` LIKE '%testing%' LIMIT 1);

DROP TEMPORARY TABLE IF EXISTS tmp_field_name;
CREATE TEMPORARY TABLE tmp_field_name
(
	Id INT NOT NULL AUTO_INCREMENT,
	FieldName varchar(255),
    FieldValue varchar(255),
    PRIMARY KEY ( Id )
);
INSERT tmp_field_name ( FieldName, FieldValue )
VALUES
	( 'UserName', @accountUserName ),
    ( 'Email', @accountEmail ),
    ( 'Password', @pword ),
    ( 'Confirm Password', @pword );
    
INSERT TESTPARAMETER ( Test, IterationId, OrderId, FieldName, FieldValue )
SELECT 
	@testName `Test`,
    @iterationId IterationId,
    fn.Id OrderId,
    fn.FieldName,
    fn.FieldValue
  FROM tmp_field_name fn
  LEFT
  JOIN TESTPARAMETER p 
    ON @testName = p.`Test` 
    AND @iterationId = p.IterationId
    AND fn.Id = p.OrderId
  WHERE	p.`Test` IS NULL
  ORDER BY fn.Id;
  
