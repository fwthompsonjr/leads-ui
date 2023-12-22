USE wlogpermissions;

CREATE TABLE `LOGCONTENT` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `RequestId` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
  `StatusId` int DEFAULT (0),
  `LineNumber` int DEFAULT (0),
  `NameSpace` varchar(255) DEFAULT NULL,
  `ClassName` varchar(255) DEFAULT NULL,
  `MethodName` varchar(255) DEFAULT NULL,
  `Message` varchar(500) DEFAULT NULL,
  `CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
  PRIMARY KEY (`Id`),
  INDEX `Idx_LogContent_CreateDate` (`CreateDate`),
  INDEX `Idx_LogContent_StatusId` (`StatusId`),
  INDEX `Idx_LogContent_NameSpace_ClassName_MethodName` (`StatusId`),
  INDEX `Idx_LogContent_RequestId_CreateDate` (`RequestId`,`CreateDate`)
);


CREATE TABLE `LOGCONTENTDETAIL` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `LogContentId` BIGINT NOT NULL REFERENCES LOGCONTENT( Id ),
  `LineId` INT,
  `Line` VARCHAR(500),
  PRIMARY KEY (`Id`),
  INDEX `Idx_LogContentDetail_LogContentId` (`LogContentId`),
  INDEX `Idx_LogContentDetail_LogContentId_LineId` (`LogContentId`, `LineId`)
);