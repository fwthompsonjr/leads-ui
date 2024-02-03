-- add locked to user permissions
SET @permissionName = 'User.Locked';
SET @permissionId = '13c82fb6-ead7-441a-a0e1-e788dc14f556';
SET @orderid = 16;
/*
INSERT PERMISSIONMAP
( Id, OrderId, KeyName)
SELECT 
	@permissionId Id, 
	@orderid OrderId, 
    @permissionName KeyName
WHERE NOT EXISTS( SELECT 1 FROM PERMISSIONMAP WHERE KeyName = @permissionName);
*/
INSERT USERPERMISSION
( Id, UserId, PermissionMapId, KeyValue )
SELECT
	concat( LEFT(id,8), "-", MID(Id,8,4), "-", MID(Id, 12,4), "-", MID(Id, 16,4), "-", RIGHT(Id,12)) Id,
    UserId,
    PermissionMapId,
    KeyValue
FROM
(
SELECT 
	md5(UUID()) Id,
	U.Id UserId,
    @permissionId PermissionMapId,
    '' KeyValue
FROM USERS U
LEFT JOIN USERPERMISSION Up
ON U.Id = Up.UserId
AND @permissionId = Up.PermissionMapId
WHERE Up.Id IS NULL
) SQ;