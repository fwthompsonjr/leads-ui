-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema defaultdb
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema defaultdb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `defaultdb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
-- -----------------------------------------------------
-- Schema wlogpermissions
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema wlogpermissions
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `wlogpermissions` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `defaultdb` ;

-- -----------------------------------------------------
-- Table `defaultdb`.`APPLICATIONS`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`APPLICATIONS` (
  `Id` CHAR(36) NOT NULL,
  `Name` VARCHAR(150) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`BGCOMPONENT`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`BGCOMPONENT` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `ComponentName` VARCHAR(75) NOT NULL,
  `ServiceName` VARCHAR(75) NOT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_bg_component_name_service` (`ComponentName` ASC, `ServiceName` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`BGCOMPONENTHEATH`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`BGCOMPONENTHEATH` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `ComponentId` CHAR(36) NOT NULL,
  `LineNbr` INT NOT NULL,
  `Health` VARCHAR(25) NULL DEFAULT NULL,
  `HealthId` INT NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_bgcomponentheath_componentid_linenbr` (`ComponentId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_bgcomponentheath_componentid_id`
    FOREIGN KEY (`ComponentId`)
    REFERENCES `defaultdb`.`BGCOMPONENT` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`BGCOMPONENTSTATUS`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`BGCOMPONENTSTATUS` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `ComponentId` CHAR(36) NOT NULL,
  `LineNbr` INT NOT NULL,
  `StatusName` VARCHAR(25) NULL DEFAULT NULL,
  `StatusId` INT NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_bgcomponentstatus_componentid_linenbr` (`ComponentId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_bgcomponentstatus_componentid_id`
    FOREIGN KEY (`ComponentId`)
    REFERENCES `defaultdb`.`BGCOMPONENT` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`PERMISSIONGROUP`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`PERMISSIONGROUP` (
  `Id` CHAR(36) NOT NULL,
  `Name` VARCHAR(50) NULL DEFAULT NULL,
  `GroupId` INT NULL DEFAULT NULL,
  `OrderId` INT NULL DEFAULT NULL,
  `PerRequest` INT NULL DEFAULT NULL,
  `PerMonth` INT NULL DEFAULT NULL,
  `PerYear` INT NULL DEFAULT NULL,
  `IsActive` TINYINT(1) NOT NULL DEFAULT true,
  `IsVisible` TINYINT(1) NOT NULL DEFAULT true,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`PERMISSIONMAP`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`PERMISSIONMAP` (
  `Id` CHAR(36) NOT NULL,
  `OrderId` INT NULL DEFAULT NULL,
  `KeyName` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`PROFILEMAP`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`PROFILEMAP` (
  `Id` CHAR(36) NOT NULL,
  `OrderId` INT NULL DEFAULT NULL,
  `KeyName` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`REASONCODES`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`REASONCODES` (
  `ReasonCode` CHAR(4) NOT NULL,
  `Reason` VARCHAR(125) NULL DEFAULT NULL,
  PRIMARY KEY (`ReasonCode`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERS`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERS` (
  `Id` CHAR(36) NOT NULL,
  `UserName` VARCHAR(50) NULL DEFAULT NULL,
  `Email` VARCHAR(255) NULL DEFAULT NULL,
  `PasswordHash` VARCHAR(150) NULL DEFAULT NULL,
  `PasswordSalt` VARCHAR(150) NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`SEARCH`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`SEARCH` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `UserId` CHAR(36) NOT NULL,
  `Name` VARCHAR(50) NULL DEFAULT NULL,
  `StartDate` DATETIME NOT NULL,
  `EndDate` DATETIME NULL DEFAULT NULL,
  `ExpectedRows` INT NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_search_src_name` (`Name` ASC) VISIBLE,
  INDEX `fk_searchrequest__users_id` (`UserId` ASC) VISIBLE,
  CONSTRAINT `fk_searchrequest__users_id`
    FOREIGN KEY (`UserId`)
    REFERENCES `defaultdb`.`USERS` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`SEARCHDETAIL`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`SEARCHDETAIL` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `SearchId` CHAR(36) NOT NULL,
  `LineNbr` INT NOT NULL,
  `Line` MEDIUMBLOB NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_SEARCHDETAIL_searchid_linenbr` (`SearchId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_searchdetail__search_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `defaultdb`.`SEARCH` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`SEARCHREQUEST`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`SEARCHREQUEST` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `SearchId` CHAR(36) NOT NULL,
  `LineNbr` INT NOT NULL,
  `Line` MEDIUMBLOB NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_searchrequest_searchid_linenbr` (`SearchId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_searchrequest__search_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `defaultdb`.`SEARCH` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`SEARCHRESPONSE`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`SEARCHRESPONSE` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `SearchId` CHAR(36) NOT NULL,
  `LineNbr` INT NOT NULL,
  `Line` MEDIUMBLOB NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_searchresponse_searchid_linenbr` (`SearchId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_searchresponse__search_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `defaultdb`.`SEARCH` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`SEARCHSTAGING`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`SEARCHSTAGING` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `SearchId` CHAR(36) NOT NULL,
  `StagingType` VARCHAR(75) NULL DEFAULT NULL,
  `LineNbr` INT NOT NULL,
  `Line` MEDIUMBLOB NULL DEFAULT NULL,
  `IsBinary` TINYINT NULL DEFAULT '0',
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_searchstaging_searchid_linenbr` (`SearchId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_searchstaging__search_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `defaultdb`.`SEARCH` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`SEARCHSTATUS`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`SEARCHSTATUS` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `SearchId` CHAR(36) NOT NULL,
  `LineNbr` INT NOT NULL,
  `Line` VARCHAR(255) NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  INDEX `idx_search_status_searchid_linenbr` (`SearchId` ASC, `CreateDate` ASC) VISIBLE,
  CONSTRAINT `fk_search_status__search_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `defaultdb`.`SEARCH` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERPERMISSION`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERPERMISSION` (
  `Id` CHAR(36) NOT NULL,
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `PermissionMapId` CHAR(36) NULL DEFAULT NULL,
  `KeyValue` VARCHAR(256) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERPERMISSIONCHANGE`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERPERMISSIONCHANGE` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `GroupId` INT NULL DEFAULT NULL,
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `ReasonCode` CHAR(4) NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERPERMISSIONHISTORY`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERPERMISSIONHISTORY` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `UserPermissionId` CHAR(36) NULL DEFAULT NULL,
  `GroupId` INT NULL DEFAULT NULL,
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `PermissionMapId` CHAR(36) NULL DEFAULT NULL,
  `KeyName` VARCHAR(100) NULL DEFAULT NULL,
  `KeyValue` VARCHAR(256) NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERPROFILE`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERPROFILE` (
  `Id` CHAR(36) NOT NULL,
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `ProfileMapId` CHAR(36) NULL DEFAULT NULL,
  `KeyValue` VARCHAR(256) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERPROFILECHANGE`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERPROFILECHANGE` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `GroupId` INT NULL DEFAULT NULL,
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `ReasonCode` CHAR(4) NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERPROFILEHISTORY`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERPROFILEHISTORY` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `UserProfileId` CHAR(36) NULL DEFAULT NULL,
  `GroupId` INT NULL DEFAULT NULL,
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `ProfileMapId` CHAR(36) NULL DEFAULT NULL,
  `KeyName` VARCHAR(100) NULL DEFAULT NULL,
  `KeyValue` VARCHAR(256) NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERTOKENS`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERTOKENS` (
  `Id` CHAR(36) NOT NULL,
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `RefreshToken` VARCHAR(256) NOT NULL DEFAULT _utf8mb4'',
  `IsActive` TINYINT(1) NOT NULL DEFAULT true,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

USE `wlogpermissions` ;

-- -----------------------------------------------------
-- Table `wlogpermissions`.`LOGCONTENT`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wlogpermissions`.`LOGCONTENT` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `RequestId` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `StatusId` INT NULL DEFAULT 0,
  `LineNumber` INT NULL DEFAULT 0,
  `NameSpace` VARCHAR(255) NULL DEFAULT NULL,
  `ClassName` VARCHAR(255) NULL DEFAULT NULL,
  `MethodName` VARCHAR(255) NULL DEFAULT NULL,
  `Message` VARCHAR(500) NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  INDEX `Idx_LogContent_CreateDate` (`CreateDate` ASC) VISIBLE,
  INDEX `Idx_LogContent_StatusId` (`StatusId` ASC) VISIBLE,
  INDEX `Idx_LogContent_NameSpace_ClassName_MethodName` (`StatusId` ASC) VISIBLE,
  INDEX `Idx_LogContent_RequestId_CreateDate` (`RequestId` ASC, `CreateDate` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 5410
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `wlogpermissions`.`LOGCONTENTDETAIL`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wlogpermissions`.`LOGCONTENTDETAIL` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `LogContentId` BIGINT NOT NULL,
  `LineId` INT NULL DEFAULT NULL,
  `Line` VARCHAR(500) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `Idx_LogContentDetail_LogContentId` (`LogContentId` ASC) VISIBLE,
  INDEX `Idx_LogContentDetail_LogContentId_LineId` (`LogContentId` ASC, `LineId` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 8
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

USE `defaultdb` ;

-- -----------------------------------------------------
-- Placeholder table for view `defaultdb`.`VWUSERPERMISSION`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`VWUSERPERMISSION` (`id` INT, `userid` INT, `permissionmapid` INT, `keyvalue` INT, `keyname` INT, `orderid` INT);

-- -----------------------------------------------------
-- Placeholder table for view `defaultdb`.`VWUSERPROFILE`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`VWUSERPROFILE` (`id` INT, `userid` INT, `profilemapid` INT, `keyvalue` INT, `keyname` INT, `orderid` INT);

-- -----------------------------------------------------
-- procedure USP_APPEND_PERMISSION_HISTORY
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_PERMISSION_HISTORY`(IN userindex char(36), IN changecode char(4))
BEGIN 
 
-- update history for current user 
UPDATE USERPERMISSIONHISTORY h   
SET GroupId = CASE WHEN GroupId IS NULL THEN 0 ELSE GroupId - 1 END   
WHERE h.UserId = userindex;  
 
-- add change history line record(s) for user   
INSERT INTO USERPERMISSIONCHANGE   
(   
UserId, GroupId, CreateDate   
)  
SELECT h.UserId, h.GroupId, h.CreateDate   
FROM   
(   
SELECT UserId, GroupId, Max( createdate ) createdate    
FROM USERPERMISSIONHISTORY   
WHERE UserId = userindex   
GROUP BY UserId, GroupId   
) h   
LEFT JOIN USERPERMISSIONCHANGE c   
ON     h.UserId = c.UserId   
AND    h.CreateDate = c.CreateDate   
WHERE  c.Id is null;  
 
-- synchronize group indexes   
UPDATE USERPERMISSIONCHANGE C 
JOIN (   
    SELECT UserId, GroupId, Max( CreateDate ) CreateDate    
    FROM USERPERMISSIONHISTORY   
    WHERE UserId = userindex   
    GROUP BY UserId, GroupId   
  ) AS subquery  
  ON C.UserId = subquery.UserId 
  AND C.CreateDate = subquery.CreateDate 
SET   C.GroupId = subquery.GroupId 
WHERE 1 = 1   
AND C.UserId = subquery.UserId   
AND C.CreateDate = subquery.CreateDate   
AND C.GroupId != subquery.GroupId; 
 
-- set change reason code 
UPDATE USERPERMISSIONCHANGE   
SET  ReasonCode = changecode   
WHERE    
EXISTS (SELECT 1 FROM REASONCODES WHERE ReasonCode = changecode)   
AND UserId = userindex   
AND GroupId = 0   
AND ReasonCode IS NULL;  
 
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_PROFILE_HISTORY
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_PROFILE_HISTORY`(IN userindex char(36), IN changecode char(4))
BEGIN 
 
-- update history for current user 
UPDATE USERPROFILEHISTORY h   
SET GroupId = CASE WHEN GroupId IS NULL THEN 0 ELSE GroupId - 1 END   
WHERE h.UserId = userindex;  
 
-- add change history line record(s) for user   
INSERT INTO USERPROFILECHANGE   
(   
UserId, GroupId, CreateDate   
)  
SELECT h.UserId, h.GroupId, h.CreateDate   
FROM   
(   
SELECT UserId, GroupId, Max( createdate ) createdate    
FROM USERPROFILEHISTORY   
WHERE UserId = userindex   
GROUP BY UserId, GroupId   
) h   
LEFT JOIN USERPROFILECHANGE c   
ON     h.UserId = c.UserId   
AND    h.CreateDate = c.CreateDate   
WHERE  c.Id is null;  
 
-- synchronize group indexes   
UPDATE USERPROFILECHANGE C 
JOIN (   
    SELECT UserId, GroupId, Max( CreateDate ) CreateDate    
    FROM USERPROFILEHISTORY   
    WHERE UserId = userindex   
    GROUP BY UserId, GroupId   
  ) AS subquery  
  ON C.UserId = subquery.UserId 
  AND C.CreateDate = subquery.CreateDate 
SET   C.GroupId = subquery.GroupId 
WHERE 1 = 1   
AND C.UserId = subquery.UserId   
AND C.CreateDate = subquery.CreateDate   
AND C.GroupId != subquery.GroupId; 
 
-- set change reason code 
UPDATE USERPROFILECHANGE   
SET  ReasonCode = changecode   
WHERE    
EXISTS (SELECT 1 FROM REASONCODES WHERE ReasonCode = changecode)   
AND UserId = userindex   
AND GroupId = 0   
AND ReasonCode IS NULL;  
 
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_USER_SEARCH_AND_REQUEST
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_USER_SEARCH_AND_REQUEST`(
	in userRefId CHAR(36), 
	in searchGuid CHAR(36), 
	in searchStart DATETIME, 
	in jscontent MEDIUMBLOB)
BEGIN
DECLARE searchItemId CHAR(36);

INSERT SEARCH
  ( UserId, `Name`, StartDate )
VALUES ( userRefId, searchGuid,  searchStart );

SET searchItemId = (SELECT Id
  FROM SEARCH
  WHERE UserId = userRefId
  ORDER BY CreateDate DESC LIMIT 1);
  
/*
Note: 	a search request is the inbound payload from the user
		a search detail is the above payload transformed for the search api
*/

INSERT SEARCHREQUEST ( SearchId, LineNbr, Line )
VALUES ( searchItemId, 1, jscontent);

SELECT Id
FROM SEARCH
WHERE UserId = userRefId
  AND `Name` = searchGuid
ORDER BY CreateDate DESC LIMIT 1;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_USER_SEARCH_DETAIL
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_USER_SEARCH_DETAIL`(
	in searchItemId CHAR(36), 
	in jscontent MEDIUMBLOB)
BEGIN
	INSERT SEARCHDETAIL ( SearchId, LineNbr, Line )
		VALUES ( searchItemId, 1, jscontent );
        
	SELECT LAST_INSERT_ID() Id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_USER_SEARCH_REQUEST
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_USER_SEARCH_REQUEST`(
	in searchItemId CHAR(36), 
	in jscontent MEDIUMBLOB)
BEGIN
	INSERT SEARCHREQUEST ( SearchId, LineNbr, Line )
		VALUES ( searchItemId, 1, jscontent );
        
	SELECT LAST_INSERT_ID() Id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_USER_SEARCH_RESPONSE
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_USER_SEARCH_RESPONSE`(
	in searchItemId CHAR(36), 
	in jscontent MEDIUMBLOB)
BEGIN
	IF EXISTS( SELECT 1 FROM SEARCHRESPONSE WHERE SearchId = searchItemId ) THEN
		UPDATE SEARCHRESPONSE
		SET Line = jscontent
		WHERE SearchId = searchItemId;
	ELSE
		INSERT SEARCHRESPONSE ( SearchId, LineNbr, Line )
		VALUES ( searchItemId, 1, jscontent );
	END IF;

	-- also add this content to staging
	CALL `USP_APPEND_USER_SEARCH_STAGING`(searchItemId, 'data-excel-content-created', jscontent, 1);

	SELECT Id
	FROM SEARCHRESPONSE WHERE SearchId = searchItemId;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_USER_SEARCH_STAGING
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_USER_SEARCH_STAGING`(
	in searchItemId CHAR(36), 
    in stagingName VARCHAR(25),
	in jscontent MEDIUMBLOB,
    in isByteArray tinyint)
BEGIN

	DECLARE lineNumber INT;
    IF NOT EXISTS( SELECT 1 FROM SEARCH WHERE Id = searchItemId ) THEN
		CALL `USP_CREATE_DEFAULT_ACCOUNT`();
        INSERT SEARCH (
        Id, UserId, Name, StartDate, EndDate, ExpectedRows, CreateDate
        ) VALUES (
			searchItemId, 
            '00000000-0000-0000-0000-000000000000',
            CONCAT('00000000-', date_format(utc_timestamp(), '%Y%m%d-%T')),
            utc_timestamp(), 
            utc_timestamp(), 
            NULL,
            utc_timestamp()
        );
    END IF;
    
	IF EXISTS( SELECT 1 FROM SEARCH WHERE Id = searchItemId ) THEN	
	
    SET lineNumber =
    ( SELECT MAX( LineNbr ) FROM SEARCHSTAGING WHERE SearchId = searchItemId);
    SET lineNumber = CASE WHEN lineNumber IS NULL THEN 1 ELSE lineNumber + 1 END;
    SET isByteArray = CASE WHEN isByteArray IS NULL THEN 0 ELSE isByteArray END;
    
    
	INSERT SEARCHSTAGING ( SearchId, StagingType, LineNbr, Line, IsBinary)
		VALUES ( searchItemId, stagingName, lineNumber, jscontent, isByteArray );
        
	SELECT 	Id
    FROM 	SEARCHSTAGING
    WHERE	SearchId = searchItemId
      AND	StagingType = stagingName
      AND	LineNbr = lineNumber;
      
	END IF;
      
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_USER_SEARCH_STATUS
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_USER_SEARCH_STATUS`(
	in searchItemId CHAR(36), 
	in jscontent MEDIUMBLOB)
BEGIN
	DECLARE nxtLineId INT;
    SET nxtLineId = (SELECT MAX( LineNbr ) FROM SEARCHSTATUS WHERE SearchId = searchItemId);
    SET nxtLineId = CASE WHEN nxtLineId IS NULL THEN 1 ELSE nxtLineId + 1 END;
	INSERT SEARCHSTATUS ( SearchId, LineNbr, Line )
		VALUES ( searchItemId, nxtLineId, jscontent );
        
	SELECT LAST_INSERT_ID() Id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_COMPONENT_GET_STATUS
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_COMPONENT_GET_STATUS`(
	in component_name varchar(75),
    in service_name varchar(75)
)
BEGIN
DECLARE has_parameter BIT;
SET 	has_parameter = CASE WHEN component_name IS NULL AND service_name IS NULL THEN 0 ELSE 1 END;
	SELECT
		S.Id, S.ComponentId, S.LineNbr, S.StatusName, S.StatusId, S.CreateDate
        FROM 	BGCOMPONENTSTATUS S 
        INNER
         JOIN	BGCOMPONENT C 
           ON	C.Id = S.ComponentId
	    WHERE 	has_parameter = 1
          AND	C.ComponentName = CASE WHEN component_name IS NULL THEN C.ComponentName ELSE component_name END
          AND	C.ServiceName = CASE WHEN service_name IS NULL THEN C.ServiceName ELSE service_name END
		ORDER 
           BY 	S.CreateDate DESC;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_COMPONENT_REPORT_HEALTH
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_COMPONENT_REPORT_HEALTH`(
	in component_name varchar(75),
    in service_name varchar(75),
    in heath_name varchar(25)
)
BEGIN
	DECLARE component_id CHAR(36);
	DECLARE health_line_nbr INT;
    IF NOT EXISTS( SELECT 1 
		FROM BGCOMPONENT 
        WHERE ComponentName = component_name
          AND ServiceName = service_name ) THEN
		INSERT BGCOMPONENT ( ComponentName, ServiceName )
        VALUES ( component_name, service_name );
    END IF;
    SET component_id = (SELECT Id FROM BGCOMPONENT 
		WHERE	ComponentName = component_name
          AND	ServiceName = service_name);
    SET health_line_nbr = (SELECT MAX( LineNbr ) FROM BGCOMPONENTHEATH WHERE ComponentId = component_id);
    SET health_line_nbr = CASE WHEN health_line_nbr IS NULL THEN 1 ELSE health_line_nbr + 1 END;
    
    INSERT BGCOMPONENTHEATH
    ( ComponentId, LineNbr, Health, HealthId )
    SELECT
    component_id `ComponentId`,
    health_line_nbr LineNbr,
	CASE 
		WHEN heath_name IN ( 'Unhealthy', 'Degraded', 'Healthy' ) THEN heath_name
		ELSE 'Unknown' END `Health`,
    CASE 
		WHEN heath_name = "Unhealthy" 	THEN 0
		WHEN heath_name = "Degraded" 	THEN 1
		WHEN heath_name = "Healthy" 	THEN 2
		ELSE -1 END `HealthId`;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_COMPONENT_SET_STATUS
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_COMPONENT_SET_STATUS`(
	in component_name varchar(75),
    in service_name varchar(75),
    in status_name varchar(25)
)
BEGIN
	DECLARE component_id CHAR(36);
	DECLARE status_line_nbr INT;
	
    IF NOT EXISTS( SELECT 1 
		FROM BGCOMPONENT 
        WHERE ComponentName = component_name
          AND ServiceName = service_name ) THEN
		INSERT BGCOMPONENT ( ComponentName, ServiceName )
        VALUES ( component_name, service_name );
    END IF;
	
    SET component_id = (SELECT Id FROM BGCOMPONENT 
		WHERE	ComponentName = component_name
          AND	ServiceName = service_name);
		  
    SET status_line_nbr = (SELECT MAX( LineNbr ) FROM BGCOMPONENTSTATUS WHERE ComponentId = component_id);
    SET status_line_nbr = CASE WHEN status_line_nbr IS NULL THEN 1 ELSE status_line_nbr + 1 END;
    
    INSERT BGCOMPONENTSTATUS
    ( ComponentId, LineNbr, StatusName, StatusId )
    SELECT
    component_id `ComponentId`,
    status_line_nbr LineNbr,
	CASE 
		WHEN status_name IN ( 'Active', 'Inactive', 'Paused' ) THEN status_name
		ELSE 'Active' END `StatusName`,
    CASE 
		WHEN status_name = "Inactive" 		THEN -1
		WHEN status_name = "Active" 		THEN 1
		WHEN status_name = "Paused" 		THEN 0
		ELSE 1 END `StatusId`;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_CREATE_DEFAULT_ACCOUNT
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_CREATE_DEFAULT_ACCOUNT`()
BEGIN
DECLARE uuidx CHAR(36) DEFAULT '00000000-0000-0000-0000-000000000000';
DECLARE pwhash VARCHAR(75) DEFAULT 'OvlCtx1sOcoB+XtR0j9DPKWrev1zpiVaiZLT6pAql/ygO2O704b97GsiaiUJALm7';
DECLARE pwsalt VARCHAR(75) DEFAULT '02jmAE1+wfAjn6bOjGnEfw==';
DECLARE uemail VARCHAR(50) DEFAULT 'default-account@temp.org';
DECLARE uname VARCHAR(50) DEFAULT 'default.account';

IF NOT EXISTS ( SELECT 1 FROM `USERS` WHERE Id = uuidx ) THEN
	INSERT `USERS`
    ( Id, UserName, Email, PasswordHash, PasswordSalt, CreateDate)
    VALUES
    ( uuidx, uname, uemail, pwhash, pwsalt, utc_timestamp() );
END IF;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_FIND_USER_SEARCH_STAGING
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_FIND_USER_SEARCH_STAGING`(
	in searchItemId CHAR(36), 
    in stagingName VARCHAR(25)
    )
BEGIN
	DECLARE lineNumber INT;
    SET lineNumber =
    ( SELECT MAX( LineNbr ) FROM SEARCHSTAGING WHERE SearchId = searchItemId AND StagingType = stagingName);
    
    SELECT 	Id, 
			SearchId, 
            StagingType, 
            LineNbr, 
            CASE WHEN IsBinary = 1 THEN Line ELSE NULL END LineData,
            CASE WHEN IsBinary = 0 THEN CONVERT(`Line` USING UTF8MB4) ELSE NULL END LineText,
            IsBinary, 
            CreateDate
    FROM 	SEARCHSTAGING
    WHERE	SearchId = searchItemId
      AND	StagingType = stagingName
      AND	LineNbr = lineNumber;
      
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_REFRESH_TOKEN
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_REFRESH_TOKEN`(IN userIndex CHAR(36), IN userToken VARCHAR(256))
BEGIN

SELECT Id, UserId, RefreshToken, IsActive, CreateDate
  FROM USERTOKENS T
  WHERE 1 = 1
  AND T.UserId = userIndex
  AND T.RefreshToken = userToken
  ORDER BY T.CreateDate DESC
  LIMIT 1;
  
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_USER_SEARCH
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_USER_SEARCH`(
	in userUid CHAR(36))
BEGIN
	SELECT 
		S.Id, 
        S.UserId, 
        S.Name, 
        S.StartDate, 
        S.EndDate, 
        S.ExpectedRows, 
        S.CreateDate
	FROM 	`defaultdb`.`SEARCH` S
	WHERE	S.UserId = userUid
	ORDER BY S.CreateDate DESC;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_USER_SEARCH_DETAIL
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_USER_SEARCH_DETAIL`(
	in userUid CHAR(36), 
	in searchUid CHAR(36))
BEGIN
	DECLARE contextName VARCHAR(25);
    DECLARE paramCount INT;
    SET paramCount = (CASE WHEN userUid IS NULL THEN 0 ELSE 1 END) + (CASE WHEN searchUid IS NULL THEN 0 ELSE 1 END);
    SET contextName = "DETAIL";
    
    
	SELECT 
		contextName `Component`,
		D.`SearchId`,
		D.`LineNbr`,
		CONVERT(`Line` USING UTF8MB4) `Line`,
		D.`CreateDate`
	FROM 	`defaultdb`.`SEARCHDETAIL` D
	INNER 
	JOIN	`defaultdb`.`SEARCH` S
	  ON	D.SearchId = S.Id	
	WHERE	S.UserId = CASE WHEN userUid IS NULL THEN S.UserId ELSE userUid END -- userUid
	  AND 	D.SearchId = CASE WHEN searchUid IS NULL THEN D.SearchId ELSE searchUid END
      AND 	paramCount >= 1
	ORDER BY D.CreateDate DESC;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_USER_SEARCH_QUEUE
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_USER_SEARCH_QUEUE`()
BEGIN

SELECT 
	S.Id, 
    S.UserId, 
    S.Name, 
    S.StartDate, 
    S.EndDate, 
    S.ExpectedRows, 
    CONVERT(`Line` USING UTF8MB4) `Payload`,
    S.CreateDate
  FROM SEARCH S
  LEFT 
  JOIN SEARCHREQUEST SR
  ON	S.Id = SR.SearchId
  WHERE S.ExpectedRows IS NULL
    AND S.EndDate IS NULL
    AND 1 = CASE WHEN SR.LineNbr Is NULL THEN 1 ELSE SR.LineNbr END
  ORDER BY S.CreateDate
  LIMIT 15;
  
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_USER_SEARCH_REQUEST
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_USER_SEARCH_REQUEST`(
	in userUid CHAR(36), 
	in searchUid CHAR(36))
BEGIN
	DECLARE contextName VARCHAR(25);
    DECLARE paramCount INT;
    SET paramCount = (CASE WHEN userUid IS NULL THEN 0 ELSE 1 END) + (CASE WHEN searchUid IS NULL THEN 0 ELSE 1 END);
    SET contextName = "REQUEST";
    
    
	SELECT 
		contextName `Component`,
		D.`SearchId`,
		D.`LineNbr`,
		CONVERT(`Line` USING UTF8MB4) `Line`,
		D.`CreateDate`
	FROM 	`defaultdb`.`SEARCHREQUEST` D
	INNER 
	JOIN	`defaultdb`.`SEARCH` S
	  ON	D.SearchId = S.Id	
	WHERE	S.UserId = CASE WHEN userUid IS NULL THEN S.UserId ELSE userUid END -- userUid
	  AND 	D.SearchId = CASE WHEN searchUid IS NULL THEN D.SearchId ELSE searchUid END
      AND 	paramCount >= 1
	ORDER BY D.CreateDate DESC;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_USER_SEARCH_RESPONSE
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_USER_SEARCH_RESPONSE`(
	in userUid CHAR(36), 
	in searchUid CHAR(36))
BEGIN
	DECLARE contextName VARCHAR(25);
    SET contextName = "RESPONSE";
    
    
	SELECT 
		contextName `Component`,
		D.`SearchId`,
		D.`LineNbr`,
		CONVERT(`Line` USING UTF8MB4) `Line`,
		D.`CreateDate`
	FROM 	`defaultdb`.`SEARCHRESPONSE` D
	INNER 
	JOIN	`defaultdb`.`SEARCH` S
	  ON	D.SearchId = S.Id	
	WHERE	S.UserId = userUid
	  AND 	D.SearchId = CASE WHEN searchUid IS NULL THEN D.SearchId ELSE searchUid END
	ORDER BY D.CreateDate DESC;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_USER_SEARCH_STAGING
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_USER_SEARCH_STAGING`(
	in userUid CHAR(36), 
	in searchUid CHAR(36))
BEGIN    
	SELECT 
		StagingType `Component`,
		D.`SearchId`,
		D.`LineNbr`,
		CONVERT(`Line` USING UTF8MB4) `Line`,
		D.`CreateDate`
	FROM 	`defaultdb`.`SEARCHSTAGING` D
	INNER 
	JOIN	`defaultdb`.`SEARCH` S
	  ON	D.SearchId = S.Id	
	WHERE	S.UserId = userUid
	  AND 	D.SearchId = CASE WHEN searchUid IS NULL THEN D.SearchId ELSE searchUid END
	  AND 	D.IsBinary = 0
	ORDER BY D.CreateDate DESC;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_USER_SEARCH_STATUS
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_USER_SEARCH_STATUS`(
	in userUid CHAR(36), 
	in searchUid CHAR(36))
BEGIN
	DECLARE contextName VARCHAR(25);
    SET contextName = "STATUS";
    
    
	SELECT 
		contextName `Component`,
		D.`SearchId`,
		D.`LineNbr`,
		D.`Line`,
		D.`CreateDate`
	FROM 	`defaultdb`.`SEARCHSTATUS` D
	INNER 
	JOIN	`defaultdb`.`SEARCH` S
	  ON	D.SearchId = S.Id	
	WHERE	S.UserId = userUid
	  AND 	D.SearchId = CASE WHEN searchUid IS NULL THEN D.SearchId ELSE searchUid END
	ORDER BY D.CreateDate DESC;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_UPDATE_USER_SEARCH_COMPLETION
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_UPDATE_USER_SEARCH_COMPLETION`(
	in searchUid CHAR(36),
    in estimatedRecords int,
    in completionDate DATETIME)
BEGIN
	UPDATE `defaultdb`.`SEARCH` S
    SET
		S.ExpectedRows = CASE WHEN estimatedRecords IS NULL THEN S.ExpectedRows ELSE estimatedRecords END,
        S.EndDate = CASE WHEN completionDate IS NULL THEN S.EndDate ELSE completionDate END
   	WHERE	S.Id = searchUid
      AND	S.EndDate IS NULL;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_UPDATE_USER_SEARCH_STAGING
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_UPDATE_USER_SEARCH_STAGING`(
	in searchItemId CHAR(36), 
    in stagingName VARCHAR(25),
	in jscontent MEDIUMBLOB)
BEGIN
	DECLARE lineNumber INT;
    SET lineNumber =
    ( SELECT MAX( LineNbr ) FROM SEARCHSTAGING WHERE SearchId = searchItemId AND StagingType = stagingName);
    
    UPDATE SEARCHSTAGING 
		SET Line = jscontent
		WHERE 
        SearchId = searchItemId 
        AND StagingType = stagingName
        AND LineNbr = lineNumber;
        
	SELECT 	Id
    FROM 	SEARCHSTAGING
    WHERE	SearchId = searchItemId
      AND	StagingType = stagingName
      AND	LineNbr = lineNumber;
      
END$$

DELIMITER ;

-- -----------------------------------------------------
-- View `defaultdb`.`VWUSERPERMISSION`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `defaultdb`.`VWUSERPERMISSION`;
USE `defaultdb`;
CREATE  OR REPLACE ALGORITHM=UNDEFINED DEFINER=`admin`@`%` SQL SECURITY DEFINER VIEW `defaultdb`.`VWUSERPERMISSION` AS select `u`.`Id` AS `id`,`u`.`UserId` AS `userid`,`u`.`PermissionMapId` AS `permissionmapid`,`u`.`KeyValue` AS `keyvalue`,`p`.`KeyName` AS `keyname`,`p`.`OrderId` AS `orderid` from (`defaultdb`.`USERPERMISSION` `u` join `defaultdb`.`PERMISSIONMAP` `p` on((`u`.`PermissionMapId` = `p`.`Id`)));

-- -----------------------------------------------------
-- View `defaultdb`.`VWUSERPROFILE`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `defaultdb`.`VWUSERPROFILE`;
USE `defaultdb`;
CREATE  OR REPLACE ALGORITHM=UNDEFINED DEFINER=`admin`@`%` SQL SECURITY DEFINER VIEW `defaultdb`.`VWUSERPROFILE` AS select `u`.`Id` AS `id`,`u`.`UserId` AS `userid`,`u`.`ProfileMapId` AS `profilemapid`,`u`.`KeyValue` AS `keyvalue`,`p`.`KeyName` AS `keyname`,`p`.`OrderId` AS `orderid` from (`defaultdb`.`USERPROFILE` `u` join `defaultdb`.`PROFILEMAP` `p` on((`u`.`ProfileMapId` = `p`.`Id`)));
USE `wlogpermissions` ;

-- -----------------------------------------------------
-- Placeholder table for view `wlogpermissions`.`VWLOG`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wlogpermissions`.`VWLOG` (`Id` INT, `RequestId` INT, `StatusId` INT, `LineNumber` INT, `NameSpace` INT, `ClassName` INT, `MethodName` INT, `Message` INT, `LineId` INT, `Line` INT, `CreateDate` INT);

-- -----------------------------------------------------
-- procedure USP_INSERT_LOG_CONTENT
-- -----------------------------------------------------

DELIMITER $$
USE `wlogpermissions`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_INSERT_LOG_CONTENT`(
    in_request_index CHAR(36),
    in_status_index INT,
    in_line_number INT,
    in_name_space varchar(255),
    in_cls_name varchar(255),
    in_method_name VARCHAR(255),
    in_message VARCHAR(500)
)
BEGIN
	-- add record
    INSERT INTO `LOGCONTENT`
	(
		`RequestId`, `StatusId`, `LineNumber`, `NameSpace`, `ClassName`, `MethodName`, `Message`
	)
	VALUES
	(
		in_request_index,
		in_status_index,
		in_line_number,
		in_name_space,
		in_cls_name,
		in_method_name,
		in_message
	);
    
    -- return identity
    SELECT LAST_INSERT_ID() `Id`;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_SEARCH_LOG
-- -----------------------------------------------------

DELIMITER $$
USE `wlogpermissions`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SEARCH_LOG`(
	in_log_id BIGINT, 
    in_request_index CHAR(36),
    in_status_index INT,
    in_name_space varchar(255),
    in_cls_name varchar(255),
    in_method_name VARCHAR(255)
)
BEGIN
	SELECT 
		c.Id, 
        c.RequestId, 
        c.StatusId, 
        c.LineNumber, 
        c.NameSpace, 
        c.ClassName, 
        c.MethodName, 
        c.Message, 
        c.LineId,
        c.Line,
        c.CreateDate
      FROM VWLOG c
      WHERE 1 = 1
      AND c.Id = CASE WHEN in_log_id IS NULL THEN c.Id ELSE in_log_id END
      AND c.RequestId = CASE WHEN in_request_index IS NULL THEN c.RequestId ELSE in_request_index END
      AND c.StatusId = CASE WHEN in_status_index IS NULL THEN c.StatusId ELSE in_status_index END
      AND c.NameSpace = CASE WHEN in_name_space IS NULL THEN c.NameSpace ELSE in_name_space END
      AND c.ClassName = CASE WHEN in_cls_name IS NULL THEN c.ClassName ELSE in_cls_name END
      AND c.MethodName = CASE WHEN in_method_name IS NULL THEN c.MethodName ELSE in_method_name END;
      
END$$

DELIMITER ;

-- -----------------------------------------------------
-- View `wlogpermissions`.`VWLOG`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `wlogpermissions`.`VWLOG`;
USE `wlogpermissions`;
CREATE  OR REPLACE ALGORITHM=UNDEFINED DEFINER=`admin`@`%` SQL SECURITY DEFINER VIEW `wlogpermissions`.`VWLOG` AS select `c`.`Id` AS `Id`,`c`.`RequestId` AS `RequestId`,`c`.`StatusId` AS `StatusId`,`c`.`LineNumber` AS `LineNumber`,`c`.`NameSpace` AS `NameSpace`,`c`.`ClassName` AS `ClassName`,`c`.`MethodName` AS `MethodName`,`c`.`Message` AS `Message`,(case when (`d`.`LineId` is null) then 0 else `d`.`LineId` end) AS `LineId`,(case when (`d`.`Line` is null) then '' else `d`.`Line` end) AS `Line`,`c`.`CreateDate` AS `CreateDate` from (`wlogpermissions`.`LOGCONTENT` `c` left join `wlogpermissions`.`LOGCONTENTDETAIL` `d` on((`c`.`Id` = `d`.`LogContentId`))) order by `c`.`Id`,(case when (`d`.`LineId` is null) then 0 else `d`.`LineId` end);

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
