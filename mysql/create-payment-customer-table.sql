/*
	DROP TABLE IF EXISTS `PAYMENTCUSTOMER`;

	CREATE TABLE `PAYMENTCUSTOMER` (
		`Id` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
		`UserId` char(36) DEFAULT NULL,
		`CustomerId` varchar(50) DEFAULT NULL,
		`Email` varchar(255) DEFAULT NULL,
		`IsTest` bool DEFAULT ( TRUE ),
		`CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
			PRIMARY KEY (`Id`),
			UNIQUE KEY `unq_paymentcustomer_customerId` (`CustomerId`),
			CONSTRAINT `fk_paymentcustomer_userId_users_id` FOREIGN KEY (`UserId`) REFERENCES `USERS` (`Id`)
	);

*/

	DROP TABLE IF EXISTS `PAYMENTSESSION`;

	CREATE TABLE `PAYMENTSESSION` (
		`Id` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
		`UserId` char(36) DEFAULT NULL,
		`InvoiceId` char(36) DEFAULT NULL,
		`SessionType` varchar(15) DEFAULT NULL,
		`SessionId` varchar(75) DEFAULT NULL,
		`IntentId` varchar(50) DEFAULT NULL,
		`ClientId` varchar(75) DEFAULT NULL,
		`ExternalId` varchar(50) DEFAULT NULL,
		`CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
			PRIMARY KEY (`Id`),
			CONSTRAINT `fk_paymentsession_userId_users_id` FOREIGN KEY (`UserId`) REFERENCES `USERS` (`Id`),
			CONSTRAINT `fk_paymentsession_invoiceId_userinvoice_id` FOREIGN KEY (`InvoiceId`) REFERENCES `USERINVOICE` (`Id`)
	);