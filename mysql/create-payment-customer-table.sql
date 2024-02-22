	DROP TABLE IF EXISTS `PAYMENTCUSTOMER`;

	CREATE TABLE `PAYMENTCUSTOMER` (
		`Id` char(36) NOT NULL,
		`UserId` char(36) DEFAULT NULL,
		`CustomerId` varchar(50) DEFAULT NULL,
		`Email` varchar(255) DEFAULT NULL,
		`IsTest` bool DEFAULT ( TRUE ),
		`CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
			PRIMARY KEY (`Id`),
			UNIQUE KEY `unq_paymentcustomer_customerId` (`CustomerId`),
			CONSTRAINT `fk_paymentcustomer_userId_users_id` FOREIGN KEY (`UserId`) REFERENCES `USERS` (`Id`)
	);

