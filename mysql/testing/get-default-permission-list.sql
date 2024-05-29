/*
	list permissions for user
*/
SET @mappingAccount = 'error-mapping-json@whoops.com';
SET @destinationAccount = 'legallead.live@mail.com';
SET @destinationId = (SELECT Id FROM defaultdb.USERS WHERE Email = @destinationAccount);
DROP TEMPORARY TABLE IF EXISTS tmp_permission_loader;
CREATE TEMPORARY TABLE tmp_permission_loader
SELECT 
	row_number() over (ORDER BY u.Email, m.KeyName) Id,
    @destinationId UserId,
	MID(UUID(),1,36)  FakeId,
	@destinationAccount Email, 
    p.PermissionMapId, 
    CASE 
		WHEN m.KeyName = ( 'Account.IsPrimary' ) THEN 'True'
		WHEN m.KeyName = ( 'Setting.MaxRecords.Per.Month' ) THEN '15'
		WHEN m.KeyName = ( 'Setting.MaxRecords.Per.Request' ) THEN '5'
		WHEN m.KeyName = ( 'Setting.MaxRecords.Per.Year' ) THEN '50'
		WHEN m.KeyName IN ( 'Account.Permission.Level', 'Setting.Pricing.Name' ) THEN 'Guest'
        WHEN m.KeyName LIKE ( 'Setting.Pricing%' ) THEN '0'
        ELSE '' END KeyValue,
    m.KeyName
  FROM defaultdb.USERPERMISSION p
  INNER
   JOIN defaultdb.PERMISSIONMAP m
     ON p.PermissionMapId = m.Id
  INNER
   JOIN defaultdb.USERS u
     ON p.UserId = u.Id
WHERE u.Email = @mappingAccount
  AND @destinationId IS NOT NULL
ORDER BY u.Email, m.KeyName;

UPDATE tmp_permission_loader SET FakeId = MID(UUID(),1,36);
SET SQL_SAFE_UPDATES = 0;

INSERT defaultdb.USERPERMISSION (
		Id, UserId, PermissionMapId, KeyValue
    )
SELECT FakeId, src.UserId, src.PermissionMapId, src.KeyValue 
FROM tmp_permission_loader src
LEFT
JOIN defaultdb.USERPERMISSION p
  ON src.UserId = p.UserId
  AND src.PermissionMapId = p.PermissionMapId
WHERE p.Id is null;

UPDATE defaultdb.USERPERMISSION p
JOIN tmp_permission_loader src
  ON src.UserId = p.UserId
  AND src.PermissionMapId = p.PermissionMapId
  SET p.KeyValue = src.KeyValue
  WHERE CASE WHEN p.KeyValue IS NULL THEN '' ELSE p.KeyValue END != src.KeyValue;