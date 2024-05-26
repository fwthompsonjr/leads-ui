/*
	append test parameter
*/
SET @testName = 'Account.Change Password - should complete successfully';
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

SET @email1 = (SELECT EmailAddress FROM testing.RANDOMEMAIL ORDER BY RAND() LIMIT 1);
SET @email2 = (SELECT EmailAddress FROM testing.RANDOMEMAIL WHERE EmailAddress != @email1 ORDER BY RAND() LIMIT 1);
SET @email3 = (SELECT EmailAddress FROM testing.RANDOMEMAIL WHERE EmailAddress != @email1 AND EmailAddress != @email2 ORDER BY RAND() LIMIT 1);

SET @hasEmail1 = CASE WHEN @iterationId != 4 THEN 1 ELSE 0 END;
SET @hasEmail2 = CASE WHEN @iterationId in (1, 2) THEN 1 ELSE 0 END;
SET @hasEmail3 = CASE WHEN @iterationId in (1, 3) THEN 1 ELSE 0 END;

SET @email1 = CASE WHEN @hasEmail1 = 1 THEN @email1 ELSE '' END;
SET @email2 = CASE WHEN @hasEmail2 = 1 THEN @email2 ELSE '' END;
SET @email3 = CASE WHEN @hasEmail3 = 1 THEN @email3 ELSE '' END;

INSERT tmp_field_name ( FieldName, FieldValue )
VALUES
	( 'UserName', @accountUserName ),
	( 'Email', @accountEmail ),
    ( 'Password', @pword ),
    ( 'Change Password', REPLACE( REPLACE( @pword, '0', '7'), '1', '5') );
    
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
  
