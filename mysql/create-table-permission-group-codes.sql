DROP TABLE IF EXISTS PERMISSIONGROUPCODES;

CREATE TABLE `PERMISSIONGROUPCODES` (
  `Id` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
  `PermissionGroupId` char(36) NOT NULL,
  `KeyName` varchar(100) DEFAULT NULL,
  `ProductCode` varchar(100) DEFAULT NULL,
  `PriceCodeAnnual` varchar(100) DEFAULT NULL,
  `PriceCodeMonthly` varchar(100) DEFAULT NULL,
  `KeyJs` varchar(500) DEFAULT NULL,
  `IsActive` bit DEFAULT (1),
  `CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
  PRIMARY KEY (`Id`),
  KEY `fk_permissiongroup__permission_group_id` (`PermissionGroupId`),
  CONSTRAINT `fk_permissiongroup__permission_group_id` FOREIGN KEY (`PermissionGroupId`) REFERENCES `PERMISSIONGROUP` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
