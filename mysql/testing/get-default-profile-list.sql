/*
	list permissions for user
*/
SET @mappingAccount = 'error-mapping-json@whoops.com';
SET @destinationAccount = 'legallead.live@mail.com';
SET @destinationId = (SELECT Id FROM defaultdb.USERS WHERE Email = @destinationAccount);
DROP TEMPORARY TABLE IF EXISTS tmp_profile_loader;
CREATE TEMPORARY TABLE tmp_profile_loader
SELECT 
	row_number() over (ORDER BY u.Email, m.KeyName) Id,
    @destinationId UserId,
	MID(UUID(),1,36)  FakeId,
	@destinationAccount Email, 
    p.ProfileMapId, 
    '' KeyValue,
    m.KeyName
  FROM defaultdb.USERPROFILE p
  INNER
   JOIN defaultdb.PROFILEMAP m
     ON p.ProfileMapId = m.Id
  INNER
   JOIN defaultdb.USERS u
     ON p.UserId = u.Id
WHERE u.Email = @mappingAccount
  AND @destinationId IS NOT NULL
ORDER BY u.Email, m.KeyName;

UPDATE tmp_profile_loader SET FakeId = MID(UUID(),1,36);
SET SQL_SAFE_UPDATES = 0;
/*
INSERT defaultdb.USERPROFILE (
		Id, UserId, ProfileMapId, KeyValue
    )*/
SELECT FakeId, src.UserId, src.ProfileMapId, src.KeyValue 
FROM tmp_profile_loader src
LEFT
JOIN defaultdb.USERPROFILE p
  ON src.UserId = p.UserId
  AND src.ProfileMapId = p.ProfileMapId
WHERE p.Id is null;

SELECT FakeId, src.UserId, src.ProfileMapId, src.KeyValue 
FROM tmp_profile_loader src
JOIN defaultdb.USERPROFILE p
  ON src.UserId = p.UserId
  AND src.ProfileMapId = p.ProfileMapId
WHERE CASE WHEN p.KeyValue IS NULL THEN '' ELSE p.KeyValue END != src.KeyValue;
/*
UPDATE defaultdb.USERPROFILE p
JOIN tmp_profile_loader src
  ON src.UserId = p.UserId
  AND src.ProfileMapId = p.ProfileMapId
  SET p.KeyValue = src.KeyValue
  WHERE CASE WHEN p.KeyValue IS NULL THEN '' ELSE p.KeyValue END != src.KeyValue; */