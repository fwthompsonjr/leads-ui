/*
	create table to store items where a search is currently processing
    so that only one processor will work a given series of requests

CREATE TABLE `SEARCHDETAIL` (
  `Id` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
  `SearchId` char(36) NOT NULL,
  `LineNbr` int NOT NULL,
  `Line` mediumblob,
  `CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
  PRIMARY KEY (`Id`),
  UNIQUE KEY `unq_SEARCHDETAIL_searchid_linenbr` (`SearchId`,`LineNbr`),
  CONSTRAINT `fk_searchdetail__search_id` FOREIGN KEY (`SearchId`) REFERENCES `SEARCH` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


*/
drop table if exists SEARCHWORKING;
create table SEARCHWORKING (
	`Id` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
	`SearchId` char(36) NOT NULL,
	`MessageId` int NOT NULL DEFAULT( 0 ),
	`StatusId` int NOT NULL DEFAULT( 0 ),
	`MachineName` varchar(256),
	`CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
    `LastUpdateDt` datetime NOT NULL DEFAULT (utc_timestamp()),
	`CompletionDate` datetime NULL,
	PRIMARY KEY (`Id`)
	, CONSTRAINT `fk_search_working__search_id` FOREIGN KEY (`SearchId`) REFERENCES `SEARCH` (`Id`)
	, CONSTRAINT `fk_search_working__message_id` FOREIGN KEY (`MessageId`) REFERENCES `SEARCHWORKINGMESSAGE` (`Id`)
	, CONSTRAINT `fk_search_working__status_id` FOREIGN KEY (`StatusId`) REFERENCES `SEARCHWORKINGSTATUS` (`Id`)
);

select *
  from SEARCHWORKING;