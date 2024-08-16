/*
	create table queueworking
*/
CREATE TABLE `QUEUEWORKING` (
  `Id` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
  `SearchId` char(36) NOT NULL,
  `Message` VARCHAR(255),
  `StatusId` int NOT NULL DEFAULT (0),
  `MachineName` varchar(256) DEFAULT NULL,
  `CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
  `LastUpdateDt` datetime NOT NULL DEFAULT (utc_timestamp()),
  `CompletionDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_queue_working__search_id` (`SearchId`),
  KEY `fk_queue_working__status_id` (`StatusId`),
  CONSTRAINT `fk_queue_working__search_id` FOREIGN KEY (`SearchId`) REFERENCES `SEARCH` (`Id`),
  CONSTRAINT `fk_queue_working__status_id` FOREIGN KEY (`StatusId`) REFERENCES `SEARCHWORKINGSTATUS` (`Id`)
);