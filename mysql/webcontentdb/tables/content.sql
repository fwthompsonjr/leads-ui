DROP TABLE CONTENT;

CREATE TABLE `CONTENT` (
  `Id` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
  `InternalId` int DEFAULT (0),
  `VersionId` int DEFAULT (0),
  `ContentName` varchar(500) DEFAULT NULL,
  `IsActive` tinyint NOT NULL DEFAULT (false),
  `IsChild` tinyint NOT NULL DEFAULT (false),
  `CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
  PRIMARY KEY (`Id`),
  INDEX Idx_Content_CreateDate ( CreateDate ),
  INDEX Idx_Content_InternalId_VersionId ( InternalId, VersionId )
);