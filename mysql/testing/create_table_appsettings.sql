/*
	create table to store app-settings
*/
USE defaultdb;
/*
DROP TABLE IF EXISTS APPSETTINGS;

CREATE TABLE APPSETTINGS(
	`Id` char(36) NOT NULL DEFAULT (cast(uuid() as char(36) charset utf8mb4)),
    `KeyName` varchar(50) NOT NULL,
    `KeyValue` varchar(255) NOT NULL,
    `Version` decimal(5,2) DEFAULT( 1.0 ) NOT NULL,
    `CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
    PRIMARY KEY (`Id`),
    INDEX idx_appsetting_keyname ( `KeyName`, `Version` DESC )
);

*/

SET @exclude_record = '-- removed --';

DROP temporary TABLE IF EXISTS tmp_load_settings;

CREATE temporary table tmp_load_settings (
    `KeyName` varchar(50) NOT NULL,
    `KeyValue` varchar(255) NOT NULL,
    `Version` decimal(5,2) DEFAULT( 1.0 ) NOT NULL);
    
INSERT tmp_load_settings ( `KeyName`, `KeyValue`, `Version` )
	VALUES
    ( 'AWS_PROD_LOAD_BALANCER_IP', '18.118.182.232', 1.1 ),
    ( 'AWS_PROD_INTERNAL_URL', 'oxfordleads.us-east-2.elasticbeanstalk.com', 1.1 ),
    ( 'AWS_PROD_EXTERNAL_URL', 'oxfordleads.com', 1.1 ),
    ( 'HTTP_REDIRECT_ENABLED', 'TRUE', 1.1 ),
    ( 'GODADDY_ACCESS_ID', @exclude_record, 1.1 ),
    ( 'GODADDY_ACCESS_SECRET', @exclude_record, 1.1 );
    
INSERT defaultdb.APPSETTINGS ( `KeyName`, `KeyValue`, `Version` )
	select s.`KeyName`, s.`KeyValue`, s.`Version`
	from tmp_load_settings s
	left
	join APPSETTINGS a
	on	s.`KeyName` = a.`KeyName`
	and s.`Version` = a.`Version`
	where 1 = 1
	  and a.Id is null
      and s.`KeyValue` != @exclude_record;

SELECT *
  FROM defaultdb.APPSETTINGS;