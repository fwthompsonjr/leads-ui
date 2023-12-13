
CREATE TABLE `CONTENTLINE` (
  `Id` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
  `ContentId` char(36) DEFAULT NULL,
  `InternalId` int DEFAULT NULL,
  `LineNbr` int DEFAULT NULL,
  `Content` varchar(500) DEFAULT NULL,
  `CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
  PRIMARY KEY (`Id`),
  INDEX Idx_ContentLine_CreateDate ( CreateDate ),
  INDEX Idx_Content_InternalId_LineNbr ( InternalId, LineNbr )
);