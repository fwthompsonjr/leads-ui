-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema aspnettds
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema aspnettds
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `aspnettds` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
-- -----------------------------------------------------
-- Schema defaultdb
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema defaultdb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `defaultdb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `aspnettds` ;

-- -----------------------------------------------------
-- Table `aspnettds`.`adv_config`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`adv_config` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `SrcName` VARCHAR(75) NULL DEFAULT NULL,
  `IsActive` BIT(1) NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_adv_config_src_name` (`SrcName` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 22
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`adv_config_detail`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`adv_config_detail` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `ConfigId` INT NOT NULL,
  `LineNbr` INT NOT NULL,
  `Line` VARCHAR(500) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_adv_config_detail_configid_linenbr` (`ConfigId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_adv_config_detail__adv_config_id`
    FOREIGN KEY (`ConfigId`)
    REFERENCES `aspnettds`.`adv_config` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 1933
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`adv_search`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`adv_search` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NULL DEFAULT NULL,
  `StartDate` DATETIME NOT NULL,
  `EndDate` DATETIME NULL DEFAULT NULL,
  `ExpectedRows` INT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_adv_search_src_name` (`Name` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`adv_search_detail`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`adv_search_detail` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `SearchId` INT NOT NULL,
  `LineNbr` INT NOT NULL,
  `Line` MEDIUMBLOB NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_adv_search_detail_searchid_linenbr` (`SearchId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_adv_search_detail__adv_config_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `aspnettds`.`adv_search` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`adv_search_request`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`adv_search_request` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `SearchId` INT NOT NULL,
  `LineNbr` INT NOT NULL,
  `Line` MEDIUMBLOB NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_adv_search_request_searchid_linenbr` (`SearchId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_adv_search_request__adv_config_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `aspnettds`.`adv_search` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`adv_search_response`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`adv_search_response` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `SearchId` INT NOT NULL,
  `SessionId` VARCHAR(50) NULL DEFAULT NULL,
  `LineNbr` INT NOT NULL,
  `Line` MEDIUMBLOB NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_adv_search_response_searchid_linenbr` (`SearchId` ASC, `LineNbr` ASC) VISIBLE,
  CONSTRAINT `fk_adv_search_response__adv_config_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `aspnettds`.`adv_search` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`adv_search_result`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`adv_search_result` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `SessionId` VARCHAR(50) NULL DEFAULT NULL,
  `SearchId` INT NOT NULL,
  `Line` MEDIUMBLOB NULL DEFAULT NULL,
  `DateCreated` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `idx_adv_search_result_searchid_datecreated` (`SearchId` ASC, `DateCreated` ASC) VISIBLE,
  CONSTRAINT `fk_adv_search_result__adv_search_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `aspnettds`.`adv_search` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`adv_search_status`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`adv_search_status` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `SessionId` VARCHAR(50) NULL DEFAULT NULL,
  `SearchId` INT NOT NULL,
  `Line` VARCHAR(255) NULL DEFAULT NULL,
  `DateCreated` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `idx_adv_search_status_searchid_linenbr` (`SearchId` ASC, `DateCreated` ASC) VISIBLE,
  CONSTRAINT `fk_adv_search_status__adv_search_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `aspnettds`.`adv_search` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 16
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`aspnetroles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`aspnetroles` (
  `Id` VARCHAR(450) CHARACTER SET 'utf8mb3' NOT NULL,
  `Name` VARCHAR(256) CHARACTER SET 'utf8mb3' NULL DEFAULT NULL,
  `NormalizedName` VARCHAR(256) CHARACTER SET 'utf8mb3' NULL DEFAULT NULL,
  `ConcurrencyStamp` VARCHAR(8000) CHARACTER SET 'utf8mb3' NULL DEFAULT NULL,
  PRIMARY KEY (`Id`(255)))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`aspnetuserextension`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`aspnetuserextension` (
  `Id` VARCHAR(450) CHARACTER SET 'utf8mb3' NOT NULL,
  `UserName` VARCHAR(256) CHARACTER SET 'utf8mb3' NULL DEFAULT NULL,
  `FirstName` VARCHAR(50) CHARACTER SET 'utf8mb3' NULL DEFAULT NULL,
  `LastName` VARCHAR(75) CHARACTER SET 'utf8mb3' NULL DEFAULT NULL,
  PRIMARY KEY (`Id`(255)),
  INDEX `FK_AspNetUserExtension_AspNetUsers` (`Id` ASC) VISIBLE,
  CONSTRAINT `FK_AspNetUserExtension_AspNetUsers`
    FOREIGN KEY (`Id`)
    REFERENCES `aspnettds`.`aspnetusers` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`aspnetusertokens`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`aspnetusertokens` (
  `UserId` VARCHAR(450) CHARACTER SET 'utf8mb3' NOT NULL,
  `LoginProvider` VARCHAR(128) CHARACTER SET 'utf8mb3' NOT NULL,
  `Name` VARCHAR(128) CHARACTER SET 'utf8mb3' NOT NULL,
  `Value` VARCHAR(8000) CHARACTER SET 'utf8mb3' NULL DEFAULT NULL,
  PRIMARY KEY (`UserId`(255), `LoginProvider`, `Name`),
  INDEX `FK_AspNetUserTokens_AspNetUsers_UserId` (`UserId` ASC) VISIBLE,
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `aspnettds`.`aspnetusers` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`bg_result_field`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`bg_result_field` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `ung_bg_result_field_name` (`Name` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 16
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`usstate`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`usstate` (
  `Id` VARCHAR(2) NOT NULL,
  `Name` VARCHAR(100) NOT NULL,
  `Actv` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_UsState_Name` (`Name` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`bg_setting`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`bg_setting` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `County` VARCHAR(50) NOT NULL,
  `StateId` VARCHAR(2) NOT NULL,
  `CaseSearchType` VARCHAR(50) NULL DEFAULT NULL,
  `CourtSeachType` VARCHAR(50) NULL DEFAULT NULL,
  `IsCriminal` BIT(1) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `fk_bg_setting__usstate` (`StateId` ASC) VISIBLE,
  CONSTRAINT `fk_bg_setting__usstate`
    FOREIGN KEY (`StateId`)
    REFERENCES `aspnettds`.`usstate` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 5
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`bg_working_request_type`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`bg_working_request_type` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_bg_working_request_type_name` (`Name` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`bg_working_status`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`bg_working_status` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 7
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`bg_working`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`bg_working` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `County` VARCHAR(50) NOT NULL,
  `StateId` VARCHAR(2) NOT NULL,
  `StatusId` INT NULL DEFAULT NULL,
  `SettingId` INT NULL DEFAULT NULL,
  `RequestTypeId` INT NULL DEFAULT NULL,
  `StartDate` DATETIME NOT NULL,
  `EndDate` DATETIME NOT NULL,
  `SearchId` INT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `fk_bg_working__bg_setting` (`SettingId` ASC) VISIBLE,
  INDEX `fk_bg_working__bg_working_status` (`StatusId` ASC) VISIBLE,
  INDEX `fk_bg_working__usstate` (`StateId` ASC) VISIBLE,
  INDEX `idx_bg_working_bg_working_request_type_id` (`RequestTypeId` ASC) VISIBLE,
  CONSTRAINT `fk_bg_working__bg_setting`
    FOREIGN KEY (`SettingId`)
    REFERENCES `aspnettds`.`bg_setting` (`Id`),
  CONSTRAINT `fk_bg_working__bg_working_request_type`
    FOREIGN KEY (`RequestTypeId`)
    REFERENCES `aspnettds`.`bg_working_request_type` (`Id`),
  CONSTRAINT `fk_bg_working__bg_working_status`
    FOREIGN KEY (`StatusId`)
    REFERENCES `aspnettds`.`bg_working_status` (`Id`),
  CONSTRAINT `fk_bg_working__usstate`
    FOREIGN KEY (`StateId`)
    REFERENCES `aspnettds`.`usstate` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 128
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`bg_result_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`bg_result_data` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `WorkId` INT NULL DEFAULT NULL,
  `RowId` INT NULL DEFAULT NULL,
  `ColId` INT NULL DEFAULT NULL,
  `FieldData` BLOB NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `ung_bg_result_data_row_col` (`WorkId` ASC, `RowId` ASC, `ColId` ASC) VISIBLE,
  INDEX `fk_bg_result_data__bg_result_field` (`ColId` ASC) VISIBLE,
  CONSTRAINT `fk_bg_result_data__bg_result_field`
    FOREIGN KEY (`ColId`)
    REFERENCES `aspnettds`.`bg_result_field` (`Id`),
  CONSTRAINT `fk_bg_result_data__bg_working`
    FOREIGN KEY (`WorkId`)
    REFERENCES `aspnettds`.`bg_working` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`cart`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`cart` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `CartId` VARCHAR(256) NOT NULL,
  `Name` VARCHAR(25) NOT NULL,
  `ProductCode` VARCHAR(25) NOT NULL,
  `Quantity` INT NOT NULL,
  `Price` DECIMAL(9,2) NOT NULL,
  `IconCode` VARCHAR(25) NOT NULL,
  `UniqueId` VARCHAR(256) NOT NULL,
  `DateCreated` DATETIME NOT NULL,
  `DateExpired` DATETIME NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_Cart_UniqueId` (`UniqueId`(255) ASC) VISIBLE,
  UNIQUE INDEX `unq_Cart_CartId_ProductCode` (`CartId`(255) ASC, `ProductCode` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`contact`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`contact` (
  `Id` VARCHAR(50) NOT NULL,
  `SessionId` VARCHAR(50) NOT NULL,
  `FirstName` VARCHAR(25) NOT NULL,
  `LastName` VARCHAR(75) NOT NULL,
  `Email` VARCHAR(255) NOT NULL,
  `Phone` VARCHAR(20) NOT NULL,
  `Subject` VARCHAR(75) NOT NULL,
  `Message` VARCHAR(255) NOT NULL,
  `ClientIp` VARCHAR(25) NULL DEFAULT NULL,
  `DateCreated` DATETIME NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`countycourtsearch`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`countycourtsearch` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `SessionId` VARCHAR(255) NOT NULL,
  `UserId` VARCHAR(255) NOT NULL,
  `County` VARCHAR(100) NOT NULL,
  `StateId` VARCHAR(2) NOT NULL,
  `StartDate` DATETIME NOT NULL,
  `EndDate` DATETIME NOT NULL,
  `Status` VARCHAR(50) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `fk_CountyCourtSearch_State` (`StateId` ASC) VISIBLE,
  INDEX `idx_CountyCourtSearch_UserId` (`UserId` ASC) VISIBLE,
  INDEX `idx_CountyCourtSearch_UserId_StateId_County` (`UserId` ASC, `StateId` ASC, `County` ASC) VISIBLE,
  CONSTRAINT `fk_CountyCourtSearch_State`
    FOREIGN KEY (`StateId`)
    REFERENCES `aspnettds`.`usstate` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`countycourtsearchparameter`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`countycourtsearchparameter` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `SearchId` INT NOT NULL,
  `Line` MEDIUMBLOB NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `fk_countycourtsearchparameter__adv_config_id` (`SearchId` ASC) VISIBLE,
  CONSTRAINT `fk_countycourtsearchparameter__adv_config_id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `aspnettds`.`countycourtsearch` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`countycourtsearchstatus`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`countycourtsearchstatus` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `SearchId` INT NOT NULL,
  `Status` VARCHAR(50) NOT NULL,
  `CreateDate` DATETIME NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `idx_CountyCourtSearchStatus_SearchId_DateCreated` (`SearchId` ASC, `CreateDate` ASC) VISIBLE,
  CONSTRAINT `fk_CountyCourtSearchStatus_CountyCourtSearch_Id`
    FOREIGN KEY (`SearchId`)
    REFERENCES `aspnettds`.`countycourtsearch` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`uscounty`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`uscounty` (
  `Id` INT NOT NULL,
  `Name` VARCHAR(100) NOT NULL,
  `StateId` VARCHAR(2) NOT NULL,
  `Actv` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_UsCounty_Name_StateId` (`StateId` ASC, `Name` ASC) VISIBLE,
  CONSTRAINT `fk_UsCounty_UsState_StateId`
    FOREIGN KEY (`StateId`)
    REFERENCES `aspnettds`.`usstate` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`mds_countysettings`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`mds_countysettings` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `CountyId` INT NOT NULL,
  `Name` VARCHAR(100) NOT NULL,
  `DefaultValue` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_mds_countysettings_countyId_name` (`CountyId` ASC, `Name` ASC) VISIBLE,
  CONSTRAINT `fk_mds_countysettings_uscounty_id`
    FOREIGN KEY (`CountyId`)
    REFERENCES `aspnettds`.`uscounty` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 18
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`mds_countysettings_detail`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`mds_countysettings_detail` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `CountyId` INT NOT NULL,
  `CountySettingId` INT NOT NULL,
  `SortOrder` INT NOT NULL,
  `Name` VARCHAR(100) NOT NULL,
  `Value` VARCHAR(100) NULL DEFAULT NULL,
  `UiVisible` BIT(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  INDEX `fk_mds_countysettings_detail_uscounty_id` (`CountyId` ASC) VISIBLE,
  INDEX `fk_mds_countysettings_detail_mds_countysettings_id` (`CountySettingId` ASC) VISIBLE,
  CONSTRAINT `fk_mds_countysettings_detail_mds_countysettings_id`
    FOREIGN KEY (`CountySettingId`)
    REFERENCES `aspnettds`.`mds_countysettings` (`Id`),
  CONSTRAINT `fk_mds_countysettings_detail_uscounty_id`
    FOREIGN KEY (`CountyId`)
    REFERENCES `aspnettds`.`uscounty` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 92
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`mds_countysettings_user`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`mds_countysettings_user` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `CountySettingId` INT NOT NULL,
  `UserName` VARCHAR(255) NOT NULL,
  `Name` VARCHAR(100) NOT NULL,
  `DefaultValue` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_mds_countysettings_countyId_name` (`CountySettingId` ASC, `UserName` ASC, `Name` ASC) VISIBLE,
  CONSTRAINT `fk_mds_countysettings_user_mds_countysettings_id`
    FOREIGN KEY (`CountySettingId`)
    REFERENCES `aspnettds`.`mds_countysettings` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`mds_court`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`mds_court` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `StateId` VARCHAR(2) NOT NULL,
  `CountyId` INT NOT NULL,
  `InternalId` INT NOT NULL,
  `Name` VARCHAR(75) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_mds_court_internal_id_name` (`CountyId` ASC, `InternalId` ASC, `Name` ASC) VISIBLE,
  INDEX `idx_mds_court_stateId_countyId` (`StateId` ASC, `CountyId` ASC) VISIBLE,
  CONSTRAINT `fk_mds_court_uscounty`
    FOREIGN KEY (`CountyId`)
    REFERENCES `aspnettds`.`uscounty` (`Id`),
  CONSTRAINT `fk_mds_court_usstate`
    FOREIGN KEY (`StateId`)
    REFERENCES `aspnettds`.`usstate` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 69
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`mds_court_address`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`mds_court_address` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `CourtId` INT NOT NULL,
  `Name` VARCHAR(50) NOT NULL,
  `FullName` VARCHAR(150) CHARACTER SET 'utf8mb3' NULL DEFAULT NULL,
  `Address` VARCHAR(255) CHARACTER SET 'utf8mb3' NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_mds_court_address_court_id` (`CourtId` ASC) VISIBLE,
  CONSTRAINT `fk_mds_court_to_mds_court_address`
    FOREIGN KEY (`CourtId`)
    REFERENCES `aspnettds`.`mds_court` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 60
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`newsletter`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`newsletter` (
  `Id` VARCHAR(50) NOT NULL,
  `SessionId` VARCHAR(50) NOT NULL,
  `FullName` VARCHAR(150) NOT NULL,
  `Email` VARCHAR(255) NOT NULL,
  `Phone` VARCHAR(20) NOT NULL,
  `ClientIp` VARCHAR(25) NULL DEFAULT NULL,
  `DateCreated` DATETIME NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_NewsLetter_Email` (`Email` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `aspnettds`.`usr_result_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aspnettds`.`usr_result_data` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `WorkId` INT NULL DEFAULT NULL,
  `RowId` INT NULL DEFAULT NULL,
  `ColId` INT NULL DEFAULT NULL,
  `FieldData` BLOB NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `ung_usr_result_data_row_col` (`WorkId` ASC, `RowId` ASC, `ColId` ASC) VISIBLE,
  INDEX `fk_usr_result_data__bg_result_field` (`ColId` ASC) VISIBLE,
  CONSTRAINT `fk_usr_result_data__adv_search`
    FOREIGN KEY (`WorkId`)
    REFERENCES `aspnettds`.`adv_search` (`Id`),
  CONSTRAINT `fk_usr_result_data__bg_result_field`
    FOREIGN KEY (`ColId`)
    REFERENCES `aspnettds`.`bg_result_field` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

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
-- Table `defaultdb`.`COURTADDRESS`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`COURTADDRESS` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `StateId` VARCHAR(5) NOT NULL,
  `CountyName` VARCHAR(75) NOT NULL,
  `CourtName` VARCHAR(50) NOT NULL,
  `FullName` VARCHAR(150) NULL DEFAULT NULL,
  `Address` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_court_address_state_county_court_name` (`StateId` ASC, `CountyName` ASC, `CourtName` ASC) VISIBLE)
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
-- Table `defaultdb`.`DISCOUNTREQUEST`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`DISCOUNTREQUEST` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `ExternalId` VARCHAR(25) NULL DEFAULT NULL,
  `InvoiceUri` VARCHAR(256) NULL DEFAULT NULL,
  `DiscountJs` VARCHAR(2000) NULL DEFAULT NULL,
  `SessionId` VARCHAR(75) NULL DEFAULT NULL,
  `IsPaymentSuccess` BIT(1) NULL DEFAULT 0,
  `CompletionDate` DATETIME NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  INDEX `fk_discountrequest_userId_users_id` (`UserId` ASC) VISIBLE,
  CONSTRAINT `fk_discountrequest_userId_users_id`
    FOREIGN KEY (`UserId`)
    REFERENCES `defaultdb`.`USERS` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`LEVELREQUEST`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`LEVELREQUEST` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `ExternalId` VARCHAR(25) NULL DEFAULT NULL,
  `InvoiceUri` VARCHAR(256) NULL DEFAULT NULL,
  `LevelName` VARCHAR(25) NULL DEFAULT NULL,
  `SessionId` VARCHAR(75) NULL DEFAULT NULL,
  `IsPaymentSuccess` BIT(1) NULL DEFAULT 0,
  `CompletionDate` DATETIME NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  INDEX `fk_levelrequest_userId_users_id` (`UserId` ASC) VISIBLE,
  CONSTRAINT `fk_levelrequest_userId_users_id`
    FOREIGN KEY (`UserId`)
    REFERENCES `defaultdb`.`USERS` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`PAYMENTCUSTOMER`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`PAYMENTCUSTOMER` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `CustomerId` VARCHAR(50) NULL DEFAULT NULL,
  `Email` VARCHAR(255) NULL DEFAULT NULL,
  `IsTest` TINYINT(1) NULL DEFAULT true,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `unq_paymentcustomer_customerId` (`CustomerId` ASC) VISIBLE,
  INDEX `fk_paymentcustomer_userId_users_id` (`UserId` ASC) VISIBLE,
  CONSTRAINT `fk_paymentcustomer_userId_users_id`
    FOREIGN KEY (`UserId`)
    REFERENCES `defaultdb`.`USERS` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`PAYMENTSESSION`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`PAYMENTSESSION` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `InvoiceId` CHAR(36) NULL DEFAULT NULL,
  `SessionType` VARCHAR(15) NULL DEFAULT NULL,
  `SessionId` VARCHAR(75) NULL DEFAULT NULL,
  `IntentId` VARCHAR(50) NULL DEFAULT NULL,
  `ClientId` VARCHAR(75) NULL DEFAULT NULL,
  `ExternalId` VARCHAR(50) NULL DEFAULT NULL,
  `JsText` MEDIUMBLOB NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  INDEX `fk_paymentsession_userId_users_id` (`UserId` ASC) VISIBLE,
  CONSTRAINT `fk_paymentsession_userId_users_id`
    FOREIGN KEY (`UserId`)
    REFERENCES `defaultdb`.`USERS` (`Id`))
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
  `ProductCode` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`PERMISSIONGROUPCODES`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`PERMISSIONGROUPCODES` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `PermissionGroupId` CHAR(36) NOT NULL,
  `KeyName` VARCHAR(100) NULL DEFAULT NULL,
  `ProductCode` VARCHAR(100) NULL DEFAULT NULL,
  `PriceCodeAnnual` VARCHAR(100) NULL DEFAULT NULL,
  `PriceCodeMonthly` VARCHAR(100) NULL DEFAULT NULL,
  `KeyJs` VARCHAR(1000) NULL DEFAULT NULL,
  `IsActive` BIT(1) NULL DEFAULT 1,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  INDEX `fk_permissiongroup__permission_group_id` (`PermissionGroupId` ASC) VISIBLE,
  CONSTRAINT `fk_permissiongroup__permission_group_id`
    FOREIGN KEY (`PermissionGroupId`)
    REFERENCES `defaultdb`.`PERMISSIONGROUP` (`Id`))
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
  `RetryCount` INT NULL DEFAULT '0',
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
-- Table `defaultdb`.`USERDOWNLOADHISTORY`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERDOWNLOADHISTORY` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `SearchId` CHAR(36) NULL DEFAULT NULL,
  `Price` DECIMAL(9,2) NULL DEFAULT NULL,
  `RowCount` INT NULL DEFAULT NULL,
  `InvoiceId` VARCHAR(256) NULL DEFAULT NULL,
  `PurchaseDate` DATETIME NULL DEFAULT NULL,
  `AllowRollback` BIT(1) NULL DEFAULT b'0',
  `RollbackCount` INT NULL DEFAULT '0',
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  INDEX `UserId` (`UserId` ASC) VISIBLE,
  INDEX `SearchId` (`SearchId` ASC) VISIBLE,
  CONSTRAINT `USERDOWNLOADHISTORY_ibfk_1`
    FOREIGN KEY (`UserId`)
    REFERENCES `defaultdb`.`USERS` (`Id`),
  CONSTRAINT `USERDOWNLOADHISTORY_ibfk_2`
    FOREIGN KEY (`SearchId`)
    REFERENCES `defaultdb`.`SEARCH` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERDOWNLOAD`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERDOWNLOAD` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `DownloadId` CHAR(36) NULL DEFAULT NULL,
  `Content` MEDIUMBLOB NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  INDEX `DownloadId` (`DownloadId` ASC) VISIBLE,
  CONSTRAINT `FK_USERDOWNLOAD_USERDOWNLOADHISTORY_DOWNLOAD_ID`
    FOREIGN KEY (`DownloadId`)
    REFERENCES `defaultdb`.`USERDOWNLOADHISTORY` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `defaultdb`.`USERINVOICE`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`USERINVOICE` (
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `LineId` INT NULL DEFAULT NULL,
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `ItemType` VARCHAR(50) NULL DEFAULT NULL,
  `ItemCount` INT NULL DEFAULT NULL,
  `UnitPrice` DECIMAL(9,2) NULL DEFAULT NULL,
  `Price` DECIMAL(9,2) NULL DEFAULT NULL,
  `ReferenceId` VARCHAR(50) NULL DEFAULT NULL,
  `ExternalId` VARCHAR(50) NULL DEFAULT NULL,
  `PurchaseDate` DATETIME NULL DEFAULT NULL,
  `IsDeleted` BIT(1) NULL DEFAULT 0,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp(),
  PRIMARY KEY (`Id`),
  INDEX `UserId` (`UserId` ASC) VISIBLE,
  CONSTRAINT `fk_userinvoice_user_id_to_user`
    FOREIGN KEY (`UserId`)
    REFERENCES `defaultdb`.`USERS` (`Id`))
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


-- -----------------------------------------------------
-- Table `defaultdb`.`tmp_subscription_listing`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `defaultdb`.`tmp_subscription_listing` (
  `SubscriptionType` VARCHAR(19) NOT NULL DEFAULT '',
  `Id` CHAR(36) NOT NULL DEFAULT cast(uuid() as char(36) charset utf8mb4),
  `UserId` CHAR(36) NULL DEFAULT NULL,
  `ExternalId` VARCHAR(25) NULL DEFAULT NULL,
  `InvoiceUri` VARCHAR(256) NULL DEFAULT NULL,
  `LevelName` VARCHAR(25) NULL DEFAULT NULL,
  `SessionId` VARCHAR(75) NULL DEFAULT NULL,
  `IsPaymentSuccess` BIT(1) NULL DEFAULT 0,
  `CompletionDate` DATETIME NULL DEFAULT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT utc_timestamp())
ENGINE = InnoDB
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
-- procedure PRC__GET_INVOICE_DESCRIPTION
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `PRC__GET_INVOICE_DESCRIPTION`(IN ss_index char(36), OUT ss_description varchar(250))
BEGIN

SET @inv_description = "Record Search : ?_county_name ?_state_abbr - ?_search_start to ?_search_end on ?_requested_date";
SET @ss_parm = (
SELECT CAST( CONVERT(`Line` USING UTF8MB4) AS JSON ) Line
  FROM SEARCHREQUEST
  WHERE SearchId = ss_index
  ORDER BY CreateDate DESC
  LIMIT 1);

SET @ss_date = (
SELECT CreateDate
  FROM SEARCHREQUEST
  WHERE SearchId = ss_index
  ORDER BY CreateDate DESC
  LIMIT 1);
  
SET @county_selector = (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.county.name" ) ));
SET @state_selector = (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.state" ) ));
SET @start_long = CAST( (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.start" ) )) AS signed );
SET @ending_long = CAST( (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.end" ) )) AS signed );
SET @start_date = from_unixtime(floor(@start_long/1000));
SET @ending_date = from_unixtime(floor(@ending_long/1000));
SET @full_description = (SELECT 
 REPLACE(
 REPLACE(
 REPLACE(
 REPLACE(
 REPLACE(@inv_description, "?_county_name", UPPER(@county_selector)),
	"?_state_abbr", UPPER ( @state_selector )),
	"?_search_start", CAST( @start_date AS DATE )),
	"?_search_end", CAST( @ending_date AS DATE )),
	"?_requested_date", @ss_date )
 ItemDescription);
SET ss_description = @full_description;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure PRC__GET_INVOICE_JSON_SUMMARY
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `PRC__GET_INVOICE_JSON_SUMMARY`(IN ui_index CHAR(36), OUT tx_json TEXT)
BEGIN

drop temporary table if exists tmp_zero_invoice;
create temporary table tmp_zero_invoice
SELECT *
  FROM USERINVOICE
  WHERE LineId = 0
    AND PurchaseDate IS NOT NULL
    AND Price = 0
    AND Id = ui_index;
SET @jdata = (    
SELECT JSON_ARRAYAGG(JSON_OBJECT(
'LineId', ui.LineId, 
'UserId', ui.UserId,
'ItemType', ui.ItemType,
'ItemCount', ui.ItemCount,
'UnitPrice', ui.UnitPrice,
'Price', ui.Price,
'ReferenceId', ui.ReferenceId,
'ExternalId', ui.ExternalId,
'PurchaseDate', ui.PurchaseDate,
'CreateDate', ui.CreateDate)) 
  FROM USERINVOICE ui
  JOIN tmp_zero_invoice t
    ON ( t.ExternalId = ui.ExternalId OR t.ReferenceId = ui.ReferenceId));

SET @search_index = (SELECT ReferenceId FROM tmp_zero_invoice LIMIT 1);
SET @external_index = (SELECT ExternalId FROM tmp_zero_invoice LIMIT 1);
SET @success_fmt = REPLACE("{0}/payment-result?sts=success&id=~1", '~1', @external_index);
CALL PRC__GET_INVOICE_DESCRIPTION( @search_index, @inv_description);
SET @jso =
-- REPLACE(somecolumn, '\"', '"')
JSON_OBJECT( 'Data', @jdata,
	'SuccessUrl', @success_fmt,
    'ExternalId',  @external_index,
    'Description', @inv_description );
    
SET tx_json = (SELECT 
	REPLACE(
    CONVERT( @jso USING utf8mb4 ), '\"', '"'));
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_PERMISSION_HISTORY
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`app-dev-account`@`%` PROCEDURE `USP_APPEND_PERMISSION_HISTORY`(IN userindex char(36), IN changecode char(4))
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
CREATE DEFINER=`app-dev-account`@`%` PROCEDURE `USP_APPEND_PROFILE_HISTORY`(IN userindex char(36), IN changecode char(4))
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
-- procedure USP_APPEND_SEARCH_INVOICE_COUNTY_DISCOUNT
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_SEARCH_INVOICE_COUNTY_DISCOUNT`(
	IN uu_index CHAR(36),
	IN ss_index CHAR(36)
)
BEGIN

SET @invoice_amount = (SELECT Price FROM USERINVOICE 
WHERE ReferenceId = ss_index
  AND IsDeleted = FALSE
  AND LineId = 0);

SET @discount_amount = (SELECT PG.PerRequest * 0.01 Discount
FROM defaultdb.PERMISSIONGROUP PG
WHERE `Name` = 'County.Discount.Pricing');

SET @ss_parm = (
SELECT CAST( CONVERT(`Line` USING UTF8MB4) AS JSON ) Line
  FROM SEARCHREQUEST
  WHERE SearchId = ss_index
  ORDER BY CreateDate DESC
  LIMIT 1);

SET @county_selector = (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.county.value" ) ));


DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_subscription_set;
CREATE TEMPORARY TABLE tb_tmp_user_subscription_set AS
SELECT 	G.KeyName, P.KeyValue
	  FROM 	USERPERMISSION P
	  INNER
	  JOIN	PERMISSIONMAP G 
		ON	P.PermissionMapId = G.Id
	  WHERE	P.UserId = uu_index
        AND G.KeyName = 'Setting.State.County.Subscriptions';
SET @is_county_active = (
SELECT INSTR( KeyValue, @county_selector ) > 0 
  FROM tb_tmp_user_subscription_set t );

DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_subscription_set;
IF ( 	@is_county_active = TRUE 
		AND @discount_amount IS NOT NULL
        AND @invoice_amount IS NOT NULL
        AND NOT EXISTS( SELECT 1 FROM USERINVOICE WHERE ReferenceId = ss_index AND LineId = 1 AND UserId = uu_index AND IsDeleted = false )
	) THEN
	INSERT USERINVOICE
	(
		LineId, UserId, ItemType, ItemCount, UnitPrice, Price, ReferenceId, IsDeleted
	)
	SELECT 	1	LineId ,
			uu_index UserId,
			'County Discount' ItemType,
			1	ItemCount,
			@discount_amount UnitPrice,
			CAST( -1 * @invoice_amount * @discount_amount AS DECIMAL(9,2) ) Price,
			ss_index ReferenceId,
			FALSE IsDeleted
	WHERE
		1 = 1
		AND @is_county_active = TRUE
		AND @discount_amount IS NOT NULL
		AND @invoice_amount IS NOT NULL
		AND NOT EXISTS( SELECT 1 FROM USERINVOICE WHERE ReferenceId = ss_index AND LineId = 1 AND UserId = uu_index AND IsDeleted = false)
		;
END IF;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_SEARCH_INVOICE_HEADER
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_SEARCH_INVOICE_HEADER`(
	IN uu_index CHAR(36), 
    IN search_index CHAR(36),
    IN max_records INT)
proc_label: BEGIN
IF EXISTS ( 
	SELECT 1 FROM USERINVOICE 
	WHERE ReferenceId = search_index 
    AND UserId = uu_index 
    AND PurchaseDate IS NOT NULL) THEN
	LEAVE proc_label;
END IF;
IF EXISTS ( SELECT 1 FROM USERINVOICE WHERE ReferenceId = search_index AND UserId = uu_index ) THEN
	UPDATE USERINVOICE
		SET IsDeleted = TRUE
        WHERE ReferenceId = search_index AND UserId = uu_index;
	DELETE 
    FROM USERINVOICE
    WHERE ReferenceId = search_index 
    AND UserId = uu_index
    AND IsDeleted = TRUE;
END IF;
	SET @accountLevel = 'Account.Permission.Level';
	SET @pricingPerRequest = 'Setting.Pricing.Per.Request';
	SET @priceMx = CAST( 0.05 AS DECIMAL(9,2) );
	SET @invoiceKey = (
		SELECT UPPER( CAST( concat(
		   char(round(rand()*25)+97),
		   char(round(rand()*25)+97),
		   char(round(rand()*25)+97),
		   char(round(rand()*25)+97)) AS CHAR))
	);
	DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_key_pricing;
	CREATE TEMPORARY TABLE tb_tmp_user_key_pricing AS
	SELECT 	G.KeyName, P.KeyValue
	  FROM 	USERPERMISSION P
	  INNER
	  JOIN	PERMISSIONMAP G 
		ON	P.PermissionMapId = G.Id
	  WHERE	P.UserId = uu_index
		AND	G.KeyName IN ( @accountLevel, @pricingPerRequest );
		
	-- get the level of this user
	SET @accountName = ( SELECT KeyValue FROM tb_tmp_user_key_pricing WHERE KeyName = @accountLevel);
	SET @accountName = CASE WHEN @accountName IS NULL THEN 'Guest' ELSE @accountName END;

	-- get the pricing level for this user
	SET @pricePerRecord = ( SELECT CAST( KeyValue AS DECIMAL(9,2) ) FROM tb_tmp_user_key_pricing WHERE KeyName = @pricingPerRequest);
	SET @pricePerRecord = CASE 
		WHEN @pricePerRecord IS NULL THEN @priceMx 
        WHEN @pricePerRecord < 0 THEN CAST( 0 AS DECIMAL(9,2) )
        ELSE CAST( (@pricePerRecord * 0.01) AS DECIMAL(9,2) ) END;
	SET @itemCount = (SELECT ExpectedRows FROM SEARCH WHERE search_index = Id);
    SET @itemCount = (SELECT LEAST(@itemCount, max_records));
	SET @lineId = (SELECT COUNT(1) FROM USERINVOICE WHERE ReferenceId = search_index AND UserId = uu_index AND IsDeleted = FALSE);
	DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_key_pricing;
	SET @invoice_external_id = 
		UPPER( 
			CONCAT( 
			SUBSTRING_INDEX(uu_index, '-', -1), '-', 
			@invoiceKey, '-',
			SUBSTRING_INDEX(search_index, '-', 1) ) );
			
    INSERT USERINVOICE (
    LineId, UserId, ItemType, ItemCount, UnitPrice, Price, ReferenceId, ExternalId
    )
	SELECT
		@lineId LineId,
		uu_index UserId,
		CONCAT( 'Search - Level: ', @accountName) ItemType,
		@itemCount ItemCount,
		@pricePerRecord UnitPrice,
		CAST( (@pricePerRecord * @itemCount) as DECIMAL(9,2)) Price,
		search_index ReferenceId,
		@invoice_external_id ExternalId;
        
	-- append discount(s), if active
    CALL `defaultdb`.`USP_APPEND_SEARCH_INVOICE_COUNTY_DISCOUNT`(uu_index, search_index);
    CALL `defaultdb`.`USP_APPEND_SEARCH_INVOICE_STATE_DISCOUNT`(uu_index, search_index);
    -- calculate sales tax
    CALL `defaultdb`.`USP_APPEND_SEARCH_INVOICE_SALES_TAX`(uu_index, search_index, @invoice_external_id);
    -- sync external index
    CALL `defaultdb`.`USP_SEARCH_INVOICE_SET_EXTERNAL_ID`(uu_index, search_index);
 
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_SEARCH_INVOICE_SALES_TAX
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_SEARCH_INVOICE_SALES_TAX`(
	IN uu_index CHAR(36), 
    IN search_index CHAR(36),
    IN external_index VARCHAR(50))
BEGIN
	SET @tax_amount = CAST( 0.085 AS DECIMAL(9,5));
	SET @tax_line_id = 1000;

	DROP TEMPORARY TABLE IF EXISTS tmp_invoice_tax_amount;
	CREATE TEMPORARY TABLE tmp_invoice_tax_amount
	SELECT
		tx.LineId,
		'Sales Tax' ItemType,
		1 ItemCount,
		@tax_amount UnitPrice,
		tx.Tax Price,
		tx.ReferenceId,
		external_index ExternalId,
		NULL PurchaseDate,
		FALSE IsDeleted
	FROM
		(SELECT 
			@tax_line_id LineId,
			ReferenceId, 
			SUM( Price ) TaxableAmount, 
			CAST( SUM( Price ) * @tax_amount AS DECIMAL(9,2)) Tax
		FROM USERINVOICE 
		WHERE IsDeleted = FALSE
		  AND LineId != @tax_line_id
		  AND ReferenceId = search_index
		  AND UserId = uu_index
		GROUP BY ReferenceId
		HAVING SUM( Price ) > 0) tx
	LEFT JOIN USERINVOICE ui
	ON 	tx.ReferenceId = ui.ReferenceId
	AND tx.LineId = ui.LineId
	WHERE ui.Id IS NULL;

	IF EXISTS( SELECT 1 FROM tmp_invoice_tax_amount) THEN
		INSERT USERINVOICE
		(
			LineId, UserId, ItemType, ItemCount, UnitPrice, Price, ReferenceId, ExternalId, PurchaseDate, IsDeleted
		)
		SELECT 
			ta.LineId, 
			uu_index UserId, 
			ta.ItemType, 
			ta.ItemCount, 
			ta.UnitPrice, 
			ta.Price, 
			ta.ReferenceId, 
			ta.ExternalId, 
			ta.PurchaseDate, 
			ta.IsDeleted
		  FROM tmp_invoice_tax_amount ta;
      END IF;
	DROP TEMPORARY TABLE IF EXISTS tmp_invoice_tax_amount;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_APPEND_SEARCH_INVOICE_STATE_DISCOUNT
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_APPEND_SEARCH_INVOICE_STATE_DISCOUNT`(
	IN uu_index CHAR(36),
	IN ss_index CHAR(36)
)
BEGIN

SET @invoice_amount = (SELECT Price FROM USERINVOICE 
WHERE ReferenceId = ss_index
  AND IsDeleted = FALSE
  AND LineId = 0);

SET @discount_amount = (SELECT PG.PerRequest * 0.01 Discount
FROM defaultdb.PERMISSIONGROUP PG
WHERE `Name` = 'State.Discount.Pricing');

SET @ss_parm = (
SELECT CAST( CONVERT(`Line` USING UTF8MB4) AS JSON ) Line
  FROM SEARCHREQUEST
  WHERE SearchId = ss_index
  ORDER BY CreateDate DESC
  LIMIT 1);

SET @state_selector = (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.state" ) ));


DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_subscription_set;
CREATE TEMPORARY TABLE tb_tmp_user_subscription_set AS
SELECT 	G.KeyName, P.KeyValue
	  FROM 	USERPERMISSION P
	  INNER
	  JOIN	PERMISSIONMAP G 
		ON	P.PermissionMapId = G.Id
	  WHERE	P.UserId = uu_index
        AND G.KeyName = 'Setting.State.Subscriptions';
SET @is_state_active = (
SELECT INSTR( KeyValue, @state_selector ) > 0 
  FROM tb_tmp_user_subscription_set t );

DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_subscription_set;
IF ( 	@is_state_active = TRUE 
		AND @discount_amount IS NOT NULL
        AND @invoice_amount IS NOT NULL
        AND NOT EXISTS( SELECT 1 FROM USERINVOICE WHERE ReferenceId = ss_index AND LineId = 2 AND UserId = uu_index AND IsDeleted = false)
	) THEN
	INSERT USERINVOICE
	(
		LineId, UserId, ItemType, ItemCount, UnitPrice, Price, ReferenceId, IsDeleted
	)
	SELECT 	2	LineId ,
			uu_index UserId,
			'State Discount' ItemType,
			1	ItemCount,
			@discount_amount UnitPrice,
			CAST( -1 * @invoice_amount * @discount_amount AS DECIMAL(9,2) ) Price,
			ss_index ReferenceId,
			FALSE IsDeleted
	WHERE
		1 = 1
		AND @is_state_active = TRUE
		AND @discount_amount IS NOT NULL
		AND @invoice_amount IS NOT NULL
		AND NOT EXISTS( SELECT 1 FROM USERINVOICE WHERE ReferenceId = ss_index AND LineId = 2 AND UserId = uu_index AND IsDeleted = false)
		;
END IF;

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
	
    IF stagingName = 'data-record-count' THEN
		CALL USP_SEARCH_SET_ESTIMATED_ROW_COUNT();
    END IF;
    
	IF stagingName = 'data-output-person-addres' THEN
		CALL USP_SEARCH_SET_FINAL_ROW_COUNT();
    END IF;
    
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
-- procedure USP_CREATE_DISCOUNTREQUEST
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_CREATE_DISCOUNTREQUEST`(IN pay_load mediumtext)
BEGIN
SET @payload = pay_load;
SET @level_name = '"LevelName":';
SET @level_index_start = length(SUBSTRING_INDEX(@payload, @level_name, 1));
SET @level_index_end = length(SUBSTRING_INDEX(@payload, '"InvoiceUri":', 1));
SET @discount_js = MID( @payload, @level_index_start, @level_index_end - @level_index_start);
SET @payload = REPLACE( @payload, @discount_js, "");
SET @discount_js = REPLACE( @discount_js, CONCAT( ",", @level_name), "");
SET @discount_js = MID( @discount_js, 2, LENGTH( @discount_js ) - 2 );
SET @payload = CAST( @payload as JSON);
INSERT DISCOUNTREQUEST
(
	UserId, ExternalId, InvoiceUri, DiscountJs, SessionId, IsPaymentSuccess, CompletionDate
)
SELECT 
	js.UserId, 
    js.ExternalId, 
    js.InvoiceUri, 
    js.LevelName, 
    js.SessionId, 
    CASE WHEN js.InvoiceUri = 'NONE' THEN TRUE ELSE FALSE END IsPaymentSuccess, 
    CASE WHEN js.InvoiceUri = 'NONE' THEN utc_timestamp() ELSE NULL END CompletionDate
FROM (
	SELECT 
	JSON_VALUE( @payload, '$.UserId') UserId
    , JSON_VALUE( @payload, '$.ExternalId') ExternalId
    , CAST( @discount_js AS CHAR(2000)) LevelName
    , JSON_VALUE( @payload, '$.InvoiceUri') InvoiceUri
    , JSON_VALUE( @payload, '$.SessionId') SessionId ) js
WHERE js.UserId IS NOT NULL
  AND js.ExternalId IS NOT NULL
  AND js.LevelName IS NOT NULL
  AND js.SessionId IS NOT NULL
  AND NOT EXISTS( SELECT 1 FROM DISCOUNTREQUEST R WHERE R.ExternalId = js.ExternalId )
  AND EXISTS( SELECT 1 FROM USERS U WHERE U.Id = js.UserId );
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_CREATE_LEVELREQUEST
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_CREATE_LEVELREQUEST`(IN pay_load mediumtext)
BEGIN
SET @payload = CAST( pay_load as JSON);
INSERT LEVELREQUEST
(
	UserId, ExternalId, InvoiceUri, LevelName, SessionId, IsPaymentSuccess, CompletionDate
)
SELECT 
	js.UserId, 
    js.ExternalId, 
    js.InvoiceUri, 
    js.LevelName, 
    js.SessionId, 
    CASE WHEN js.InvoiceUri = 'NONE' THEN TRUE ELSE FALSE END IsPaymentSuccess, 
    CASE WHEN js.InvoiceUri = 'NONE' THEN utc_timestamp() ELSE NULL END CompletionDate
FROM (
	SELECT 
	JSON_VALUE( @payload, '$.UserId') UserId
    , JSON_VALUE( @payload, '$.ExternalId') ExternalId
    , JSON_VALUE( @payload, '$.LevelName') LevelName
    , JSON_VALUE( @payload, '$.InvoiceUri') InvoiceUri
    , JSON_VALUE( @payload, '$.SessionId') SessionId ) js
WHERE js.UserId IS NOT NULL
  AND js.ExternalId IS NOT NULL
  AND js.LevelName IS NOT NULL
  AND js.SessionId IS NOT NULL
  AND NOT EXISTS( SELECT 1 FROM LEVELREQUEST R WHERE R.ExternalId = js.ExternalId )
  AND EXISTS( SELECT 1 FROM USERS U WHERE U.Id = js.UserId );
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_CREATE_PAYMENT_CUSTOMER
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_CREATE_PAYMENT_CUSTOMER`(
	IN user_index CHAR(36), 
    IN account_type VARCHAR(50),
    IN customer_index VARCHAR(50)
    )
BEGIN

SET @is_test = CASE WHEN @account_type = 'test' THEN TRUE ELSE FALSE END;

IF NOT EXISTS ( SELECT 1
		FROM PAYMENTCUSTOMER pc
		JOIN
		( SELECT Id, Email
		  FROM USERS
		  WHERE Id = user_index ) ue
		ON pc.UserId = ue.Email ) THEN
    
    INSERT PAYMENTCUSTOMER ( UserId, CustomerId, Email, IsTest )    
	SELECT 
		ue.Id UserId, 
		customer_index CustomerId, 
		ue.Email, 
		@is_test IsTest
		FROM ( SELECT Id, Email
		  FROM USERS
		  WHERE Id = user_index ) ue;
END IF;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_CREATE_PRICING_TEMPLATE
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_CREATE_PRICING_TEMPLATE`(
	IN permission_code_index VARCHAR(50),
    IN pricing_json VARCHAR(500))
BEGIN
SET @currentDate = utc_timestamp();
SET @permission_group_index = (
SELECT PermissionGroupId
  FROM PERMISSIONGROUPCODES pgc
  WHERE pgc.Id = permission_code_index
    AND IsActive = FALSE);

IF   @permission_group_index IS NOT NULL THEN

	INSERT INTO `defaultdb`.`PERMISSIONGROUPCODES`
	(
	`PermissionGroupId`,
	`KeyName`,
	`ProductCode`,
	`PriceCodeAnnual`,
	`PriceCodeMonthly`,
	`KeyJs`,
	`IsActive`
	)
	SELECT
		js.`PermissionGroupId`,
		pg.`KeyName`,
		js.`ProductCode`,
		js.`PriceCodeAnnual`,
		js.`PriceCodeMonthly`,
		pricing_json KeyJs,
		FALSE IsActive
	FROM (
		SELECT @permission_group_index PermissionGroupId,
		JSON_UNQUOTE( JSON_EXTRACT( pricing_json, '$.product.code' ) ) ProductCode,
		JSON_UNQUOTE( JSON_EXTRACT( pricing_json, '$.pricecode.annual' ) ) PriceCodeAnnual,
		JSON_UNQUOTE( JSON_EXTRACT( pricing_json, '$.pricecode.monthly' ) ) PriceCodeMonthly
		) js
	JOIN ( SELECT * FROM PERMISSIONGROUPCODES WHERE Id = permission_code_index LIMIT 1 ) pg
	ON	js.PermissionGroupId = pg.PermissionGroupId
	WHERE	1 = 1
	  AND	@permission_group_index IS NOT NULL
	  AND	js.ProductCode IS NOT NULL
	  AND	js.PriceCodeAnnual IS NOT NULL
	  AND	js.PriceCodeMonthly IS NOT NULL;
	SET @pgc_id = (SELECT Id FROM PERMISSIONGROUPCODES WHERE CreateDate > @currentDate ORDER BY CreateDate LIMIT 1);
	SELECT *
    FROM PERMISSIONGROUPCODES
    WHERE Id = @pgc_id;
ELSE 
	
	SELECT *
    FROM PERMISSIONGROUPCODES
    WHERE 1 = 0;
    
END IF;
  
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_CREATE_SESSION_FOR_ZERO_INVOICES
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_CREATE_SESSION_FOR_ZERO_INVOICES`()
prc_label: BEGIN
	IF NOT EXISTS( SELECT 1 
	  FROM USERINVOICE ui
	  LEFT
	  JOIN PAYMENTSESSION ps
		ON ui.ExternalId = ps.ExternalId
	  WHERE 1 = 1
		AND ps.Id IS NULL
		AND ui.LineId = 0
		AND ui.PurchaseDate IS NOT NULL
		AND ui.Price = 0 ) THEN
        LEAVE prc_label;
	END IF;


	SET sql_safe_updates = 0;
	drop temporary table if exists tmp_zz_invoice;
	create temporary table tmp_zz_invoice
	SELECT ui.*, FALSE IsHandled
	  FROM USERINVOICE ui
	  LEFT
	  JOIN PAYMENTSESSION ps
		ON ui.ExternalId = ps.ExternalId
	  WHERE 1 = 1
		AND ps.Id IS NULL
		AND ui.LineId = 0
		AND ui.PurchaseDate IS NOT NULL
		AND ui.Price = 0;

backfill: WHILE EXISTS( SELECT 1 FROM tmp_zz_invoice WHERE IsHandled = FALSE ) DO
    SET @inv_current = (SELECT Id FROM tmp_zz_invoice WHERE IsHandled = FALSE ORDER BY PurchaseDate LIMIT 1);
    IF ( @inv_current IS NULL ) THEN
		LEAVE backfill;
	END IF;
    
    CALL PRC__GET_INVOICE_JSON_SUMMARY( @inv_current, @js_data );
    SET @jsfinal= (
			REPLACE ( 
			REPLACE ( 
			REPLACE ( CONVERT ( @js_data USING utf8mb4 ) , 
				'\\', '' ),
                '\"[', '[' ),
                ']\"', ']' )
    );
	INSERT PAYMENTSESSION
    (
		UserId, InvoiceId, ExternalId, 
        SessionType, SessionId, IntentId, ClientId,
        JsText
    )
    SELECT 	UserId, ReferenceId, ExternalId, 
			'none', 'none', 'none', 'none', @jsfinal
    FROM	tmp_zz_invoice t
    WHERE 	t.Id = @inv_current;
    
    UPDATE 	tmp_zz_invoice 
    SET 	IsHandled = TRUE
    WHERE 	Id = @inv_current;
    
END WHILE;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_FIND_DISCOUNTREQUEST_BY_EXTERNAL_INDEX
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_FIND_DISCOUNTREQUEST_BY_EXTERNAL_INDEX`(IN external_id VARCHAR(36))
BEGIN
	SELECT 
		Id, UserId, ExternalId, InvoiceUri, 
		DiscountJs LevelName, 
        SessionId, IsPaymentSuccess, CompletionDate, CreateDate
      FROM DISCOUNTREQUEST
      WHERE ExternalId = external_id
      ORDER BY CreateDate DESC
      LIMIT 1;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_FIND_DISCOUNTREQUEST_BY_USER_ID
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_FIND_DISCOUNTREQUEST_BY_USER_ID`(IN user_index CHAR(36))
BEGIN
	SELECT 
		Id, UserId, ExternalId, InvoiceUri, 
		DiscountJs LevelName, 
        SessionId, IsPaymentSuccess, CompletionDate, CreateDate
      FROM DISCOUNTREQUEST
      WHERE UserId = user_index
      ORDER BY CreateDate DESC;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_FIND_LEVELREQUEST_BY_EXTERNAL_INDEX
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_FIND_LEVELREQUEST_BY_EXTERNAL_INDEX`(IN external_id VARCHAR(36))
BEGIN
	SELECT *
      FROM LEVELREQUEST
      WHERE ExternalId = external_id
      ORDER BY CreateDate DESC
      LIMIT 1;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_FIND_LEVELREQUEST_BY_USER_ID
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_FIND_LEVELREQUEST_BY_USER_ID`(IN user_index CHAR(36))
BEGIN
	SELECT *
      FROM LEVELREQUEST
      WHERE UserId = user_index
      ORDER BY CreateDate DESC;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_FIND_PAYMENT_CUSTOMER
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_FIND_PAYMENT_CUSTOMER`(IN user_index CHAR(36), IN account_type VARCHAR(50))
BEGIN

SET @is_test = CASE WHEN @account_type = 'test' THEN TRUE ELSE FALSE END;

SELECT 
	pc.Id, 
	pc.UserId, 
    pc.CustomerId, 
    pc.Email, 
    pc.IsTest, 
    pc.CreateDate
	FROM PAYMENTCUSTOMER pc
    JOIN
	( SELECT Id, Email
	  FROM USERS
	  WHERE Id = user_index ) ue
	ON pc.UserId = ue.Id
ORDER BY pc.CreateDate
LIMIT 1;
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
-- procedure USP_GENERATE_DOWNLOAD_HISTORY_BY_SEARCH_INDEX
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GENERATE_DOWNLOAD_HISTORY_BY_SEARCH_INDEX`(in search_index char(36), in excel_content mediumblob)
BEGIN
	SET @rcount = (SELECT ExpectedRows FROM SEARCH WHERE Id = search_index);
	SET @user_index = (SELECT UserId FROM SEARCH WHERE Id = search_index);
	SET @invoice_id = (SELECT Id FROM USERINVOICE WHERE ReferenceId = search_index AND LineId = 0);
	SET @purchase_date = (SELECT Id FROM USERINVOICE WHERE ReferenceId = search_index AND LineId = 0);
	-- IF NOT EXISTS( SELECT 1 FROM USERDOWNLOADHISTORY WHERE SearchId = search_index) THEN
	INSERT USERDOWNLOADHISTORY
	(
		UserId, SearchId, Price, RowCount, InvoiceId, PurchaseDate
	)
		SELECT 
		@user_index UserId, 
		search_index SearchId, 
		SUM( Price ) Price, 
		@rcount RowCount, 
		@invoice_id InvoiceId, 
		@purchase_date PurchaseDate
	  FROM USERINVOICE
	  WHERE ReferenceId = search_index
		AND NOT EXISTS( SELECT 1 FROM USERDOWNLOADHISTORY WHERE SearchId = search_index )
	  GROUP BY  ReferenceId;

	UPDATE USERDOWNLOADHISTORY hh
		JOIN USERINVOICE ui
		ON hh.SearchId = ui.ReferenceId
		SET 
        hh.PurchaseDate = ui.PurchaseDate,
        hh.AllowRollback = FALSE
		WHERE	1 = 1
          AND	hh.SearchId = search_index 
          AND	ui.LineId = 0
		  AND 	ui.PurchaseDate is not null
		  AND 	( hh.PurchaseDate is null
					OR hh.PurchaseDate != ui.PurchaseDate);
	SET @download_id = (SELECT Id 
		FROM USERDOWNLOADHISTORY WHERE SearchId = search_index
		ORDER BY CreateDate DESC LIMIT 1);
        
	IF (	excel_content IS NOT NULL 
			AND @download_id IS NOT NULL 
            AND NOT EXISTS( SELECT 1 FROM USERDOWNLOAD WHERE DownloadId = @download_id ) ) THEN
            
			INSERT USERDOWNLOAD ( DownloadId, Content )
			VALUES ( @download_id, excel_content );
            
    END IF;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_ACTIVE_SEARCH_OVERVIEW
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_ACTIVE_SEARCH_OVERVIEW`(IN search_index VARCHAR(50))
BEGIN

SET @user_index = (SELECT UserId FROM SEARCH WHERE Id = search_index);
-- active searches
DROP TEMPORARY TABLE IF EXISTS tmp_active_searches;
CREATE TEMPORARY TABLE tmp_active_searches
SELECT Q.*,
        CASE WHEN SearchProgress != "3 - Completed" THEN SearchProgress
			 WHEN SearchProgress = 3 
			 AND EXISTS ( SELECT 1 FROM USERINVOICE WHERE ReferenceId = Q.Id AND PurchaseDate IS NOT NULL AND IsDeleted = FALSE)
			 THEN "4 - Purchased"
			 WHEN SearchProgress = 3 
			 AND EXISTS ( SELECT 1 FROM USERINVOICE WHERE ReferenceId = Q.Id AND PurchaseDate IS NOT NULL AND IsDeleted = FALSE)
			 AND EXISTS ( SELECT 1 FROM USERDOWNLOADHISTORY WHERE SearchId = Q.Id)
			 THEN "5 - Downloaded"
			 ELSE SearchProgress END SearchStatus
FROM (

	SELECT S.*,
			CASE 
				WHEN EndDate is null AND ExpectedRows is NULL THEN "1 - Submitted"
				WHEN EndDate is null AND ExpectedRows is not NULL THEN "2 - Processing"
				WHEN EndDate is not null AND ExpectedRows is not NULL AND ExpectedRows > 0 THEN "3 - Completed"
				ELSE "9 - Error" END SearchProgress
	  FROM SEARCH S
	  WHERE UserId = @user_index ) Q
    ;
  
DELETE FROM tmp_active_searches
WHERE LEFT( SearchStatus, 1 ) NOT IN ( "1", "2", "3" );

SET @searched = (
SELECT 
	JSON_ARRAYAGG(
		JSON_OBJECT(
			"id", SS.Id, 
            "startDate", SS.StartDate, 
            "endDate", SS.EndDate, 
            "expectedRows", SS.ExpectedRows, 
            "createDate", SS.CreateDate,
            "status", SS.SearchStatus)) Search
FROM tmp_active_searches SS);

SET @statuses = (
SELECT 
	JSON_ARRAYAGG(
		JSON_OBJECT(
			"id", SS.Id, 
            "searchId", SS.SearchId, 
            "lineNbr", SS.LineNbr, 
            "line", SS.Line, 
            "createDate", SS.CreateDate)) SearchStatus
  FROM SEARCHSTATUS SS
  INNER
   JOIN tmp_active_searches S ON S.Id = SS.SearchId
  ORDER BY S.CreateDate, SS.CreateDate);
  
SET @staged = (
SELECT 
	JSON_ARRAYAGG(
		JSON_OBJECT(
			"id", SS.Id, 
            "searchId", SS.SearchId, 
            "stagingType", SS.StagingType, 
            "lineNbr", SS.LineNbr, 
            "createDate", SS.CreateDate)) Search
  FROM SEARCHSTAGING SS
  INNER
   JOIN tmp_active_searches S ON S.Id = SS.SearchId
  ORDER BY S.CreateDate, SS.CreateDate);
  
SELECT 
	@searched Searches,
    @statuses Statuses,
    @staged Staged;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_ACTIVE_SEARCH_PARAMETER_DETAILS
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_ACTIVE_SEARCH_PARAMETER_DETAILS`(IN user_index VARCHAR(50))
BEGIN
SELECT Qq.*
FROM
(
	SELECT
		Q.Id,
		Q.CreateDate,
		UPPER( JSON_UNQUOTE( JSON_EXTRACT( Js, "$.county.name" ) ) ) CountyName,
		UPPER( JSON_UNQUOTE( JSON_EXTRACT( Js, "$.state" ) ) ) StateName,
		CAST( from_unixtime(CAST( JSON_UNQUOTE( JSON_EXTRACT( Js, "$.start" ) ) AS SIGNED)/1000) AS DATE ) StartDate,
		CAST( from_unixtime(CAST( JSON_UNQUOTE( JSON_EXTRACT( Js, "$.end" ) ) AS SIGNED)/1000) AS DATE )  EndDate,
			CASE WHEN SearchProgress != "3 - Completed" THEN SearchProgress
				 WHEN SearchProgress = 3 
				 AND EXISTS ( SELECT 1 FROM USERINVOICE WHERE ReferenceId = Q.Id AND PurchaseDate IS NOT NULL AND IsDeleted = FALSE)
				 THEN "4 - Purchased"
				 WHEN SearchProgress = 3 
				 AND EXISTS ( SELECT 1 FROM USERINVOICE WHERE ReferenceId = Q.Id AND PurchaseDate IS NOT NULL AND IsDeleted = FALSE)
				 AND EXISTS ( SELECT 1 FROM USERDOWNLOADHISTORY WHERE SearchId = Q.Id)
				 THEN "5 - Downloaded"
				 ELSE SearchProgress END SearchProgress
	FROM	(
	SELECT S.Id, 
		S.CreateDate,
		CAST( CONVERT( Line using utf8mb3 ) AS JSON ) Js,
			CASE 
				WHEN EndDate is null AND ExpectedRows is NULL THEN "1 - Submitted"
				WHEN EndDate is null AND ExpectedRows is not NULL THEN "2 - Processing"
				WHEN EndDate is not null AND ExpectedRows is not NULL AND ExpectedRows > 0 THEN "3 - Completed"
				ELSE "999 - Error" END SearchProgress
	  FROM SEARCHREQUEST R
	  JOIN SEARCH S 
		ON R.SearchId = S.Id
	 WHERE S.UserId = user_index
	 ) Q
 ) Qq
 WHERE LEFT( Qq.SearchProgress, 1 ) IN ( "1", "2", "3" )
 ORDER BY Qq.CreateDate;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_INVOICE_DESCRIPTION
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_INVOICE_DESCRIPTION`(IN ss_index char(36))
BEGIN

SET @inv_description = "Record Search : ?_county_name ?_state_abbr - ?_search_start to ?_search_end on ?_requested_date";
SET @ss_parm = (
SELECT CAST( CONVERT(`Line` USING UTF8MB4) AS JSON ) Line
  FROM SEARCHREQUEST
  WHERE SearchId = ss_index
  ORDER BY CreateDate DESC
  LIMIT 1);

SET @ss_date = (
SELECT CreateDate
  FROM SEARCHREQUEST
  WHERE SearchId = ss_index
  ORDER BY CreateDate DESC
  LIMIT 1);
  
SET @county_selector = (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.county.name" ) ));
SET @state_selector = (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.state" ) ));
SET @start_long = CAST( (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.start" ) )) AS signed );
SET @ending_long = CAST( (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.end" ) )) AS signed );
SET @start_date = from_unixtime(floor(@start_long/1000));
SET @ending_date = from_unixtime(floor(@ending_long/1000));
SELECT 
 REPLACE(
 REPLACE(
 REPLACE(
 REPLACE(
 REPLACE(@inv_description, "?_county_name", UPPER(@county_selector)),
	"?_state_abbr", UPPER ( @state_selector )),
	"?_search_start", CAST( @start_date AS DATE )),
	"?_search_end", CAST( @ending_date AS DATE )),
	"?_requested_date", @ss_date )
 ItemDescription,
UPPER( @county_selector ) County,
UPPER ( @state_selector ) StateAbbr,
CAST( @start_date AS DATE ) StartingDt, -- from_unixtime(floor(@start_long/1000))
CAST( @ending_date AS DATE ) EndingDt,
@ss_date RequestDt;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_PAYMENT_SESSION
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_PAYMENT_SESSION`(in external_id VARCHAR(50))
BEGIN
	SELECT 
    Id, UserId, InvoiceId, SessionType, SessionId, 
    IntentId, ClientId, ExternalId, 
    CONVERT(`JsText` USING UTF8MB4) JsText, 
    CreateDate
    FROM PAYMENTSESSION
    WHERE ExternalId = external_id
    ORDER BY CreateDate DESC
    LIMIT 1;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_PRICING_TEMPLATES
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_PRICING_TEMPLATES`()
BEGIN
SELECT pg.*
  FROM	PERMISSIONGROUPCODES pg
  JOIN	(
    SELECT KeyName, IsActive, MAX( CreateDate ) CreateDate
	FROM PERMISSIONGROUPCODES
    GROUP BY KeyName, IsActive
		) agg
	ON	pg.KeyName = agg.KeyName
    AND pg.IsActive = agg.IsActive
    AND pg.CreateDate = agg.CreateDate
ORDER BY pg.KeyName, pg.CreateDate DESC;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_PRICING_TEMPLATES_HISTORY
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_PRICING_TEMPLATES_HISTORY`()
BEGIN

SELECT pg.*
  FROM	PERMISSIONGROUPCODES pg
ORDER BY pg.KeyName, pg.CreateDate DESC;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_PURCHASE_DETAIL_BY_EXTERNAL_ID
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_PURCHASE_DETAIL_BY_EXTERNAL_ID`(IN external_index VARCHAR(50))
BEGIN
	SET @reference_index = (
	SELECT ReferenceId 
	  FROM USERINVOICE ii
	  WHERE ii.ExternalId = external_index
		AND IsDeleted = FALSE ORDER
		BY LineId LIMIT 1);
	SET @item_type = (SELECT ItemType FROM USERINVOICE 
		WHERE ReferenceId = @reference_index
		AND IsDeleted = FALSE
		AND LineId = 0
		LIMIT 1);
	SELECT
		ExternalId, Price, ItemType, PurchaseDate, UserName, Email
	FROM
	(
		SELECT 
		MAX( UserId ) UserId,
		SUM( Price ) Price,
		@item_type ItemType,
		external_index ExternalId,
		MAX( PurchaseDate ) PurchaseDate
	  FROM USERINVOICE ii
	  WHERE 1 = 1
		AND ReferenceId = @reference_index
		AND IsDeleted = FALSE
	) ii
	INNER JOIN USERS uu ON ii.UserId = uu.Id;

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
-- procedure USP_GET_SEARCH_INVOICE
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_SEARCH_INVOICE`(
	IN uu_index CHAR(36), 
	search_index CHAR(36)
)
BEGIN

	SELECT Ii.*
	FROM USERINVOICE Ii
	WHERE Ii.ReferenceId = CASE WHEN search_index IS NULL THEN Ii.ReferenceId ELSE search_index END
	AND Ii.UserId = uu_index 
	AND Ii.IsDeleted = false
	ORDER BY Ii.ReferenceId, Ii.LineId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_SEARCH_RECORD_FINAL_LIST
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_SEARCH_RECORD_FINAL_LIST`( IN search_index VARCHAR(50))
BEGIN

SET @ss_parm = (
SELECT CAST( CONVERT(`Line` USING UTF8MB4) AS JSON ) Line
  FROM SEARCHREQUEST
  WHERE SearchId = search_index
  ORDER BY CreateDate DESC
  LIMIT 1);

  
SET @county_name = (SELECT JSON_UNQUOTE( JSON_EXTRACT( @ss_parm, "$.county.name" ) ));


SET @search_staging_id = (
SELECT Id
  FROM SEARCHSTAGING
  WHERE SearchId = search_index
    AND StagingType LIKE 'data-output-person-addres%'
    ORDER BY LineNbr DESC
    LIMIT 1);

SET @txt = (
SELECT
CASE WHEN IsBinary = 0 THEN CONVERT(`Line` USING UTF8MB4) ELSE NULL END LineText
FROM defaultdb.SEARCHSTAGING
WHERE ID = @search_staging_id);
SET @jstable = CAST( @txt as JSON);

DROP TEMPORARY TABLE IF EXISTS tb_tmp_search_address_list;
CREATE TEMPORARY TABLE tb_tmp_search_address_list
SELECT SearchId, 
`Name`, 
Zip, 
Address1, 
Address2, 
Address3, 
CaseNumber, 
DateFiled, 
Court, 
CaseType, 
CaseStyle, 
FirstName, 
LastName, 
Plantiff, 
`Status`
  FROM (

SELECT search_index SearchId,
	CASE 
    WHEN STR_TO_DATE( get_ddi.DateFiled, "%m/%d/%Y") IS NULL THEN '1900-01-01'
    ELSE STR_TO_DATE( get_ddi.DateFiled, "%m/%d/%Y") END
    DtFiled,
	get_ddi.*
FROM
JSON_TABLE(@jstable, '$[*]' COLUMNS (
                `Name` VARCHAR(150)  PATH '$.Name',
                `Zip` VARCHAR(150)  PATH '$.Zip',
                `Address1` VARCHAR(150)  PATH '$.Address1',
                `Address2` VARCHAR(150)  PATH '$.Address2',
                `Address3` VARCHAR(150)  PATH '$.Address3',
                `CaseNumber` VARCHAR(150)  PATH '$.CaseNumber',
                `DateFiled` VARCHAR(150)  PATH '$.DateFiled', -- this field is blank
                `Court` VARCHAR(150)  PATH '$.Court', -- this is potentially incorrect, always same value
                `CaseType` VARCHAR(150)  PATH '$.CaseType', -- case type is actually showing the date-filed
                `CaseStyle` VARCHAR(150)  PATH '$.CaseStyle', -- this item is always the first case in dataset
                `FirstName` VARCHAR(150)  PATH '$.FirstName',
                `LastName` VARCHAR(150)  PATH '$.LastName',
                `Plantiff` VARCHAR(150)  PATH '$.Plantiff',
                `Status` VARCHAR(150)  PATH '$.Status' -- this field is blank
                )
     ) get_ddi
     ) a
     ORDER BY DtFiled, CaseNumber;
     
SELECT 
	SearchId, 
		`Name`, 
		CASE WHEN Zip = '00000' AND Address1 != 'No Address Found' THEN RIGHT( LTRIM( RTRIM( Address1 ) ), 5 ) ELSE Zip END Zip, 
		CASE WHEN Zip = '00000' AND Address1 != 'No Address Found' THEN LEFT( Address1, POSITION( ',' IN Address1 ) - 1) ELSE Address1 END Address1, 
        CASE WHEN LENGTH( Address3 ) = 0 AND LENGTH( Address2 ) != 0 THEN '' ELSE Address2 END Address2, 
        CASE 
			WHEN LENGTH( Address3 ) = 0 AND LENGTH( Address2 ) != 0 THEN Address2 
            WHEN Zip = '00000' AND Address1 != 'No Address Found' THEN TRIM(MID( Address1, POSITION( ',' IN Address1 ) + 1, LENGTH( Address1) ))
            ELSE Address3 END Address3, 
    CaseNumber, DateFiled, Court, CaseType, CaseStyle, 
    FirstName, LastName, Plantiff, 
    -- `Status`, -- this field is not populated correctly from searches excluded from data.
    County, 
    CASE WHEN CourtAddress IS NULL THEN '' ELSE CourtAddress END CourtAddress
FROM
(     
SELECT 
	aa.*,
	UPPER( @county_name ) County,
    (SELECT ca.Address
	  FROM aspnettds.mds_court_address ca
	  WHERE 
	( ca.`Name` = aa.Court  OR ca.FullName = aa.Court ) LIMIT 1) CourtAddress
    
  FROM tb_tmp_search_address_list aa
  ) q;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_SEARCH_RECORD_PREVIEW
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_SEARCH_RECORD_PREVIEW`(IN search_index CHAR(36))
BEGIN
SET @search_staging_id = (
SELECT Id
  FROM SEARCHSTAGING
  WHERE SearchId = search_index
    AND StagingType LIKE 'data-output-person-addres%'
    ORDER BY LineNbr DESC
    LIMIT 1);

SET @txt = (
SELECT
CASE WHEN IsBinary = 0 THEN CONVERT(`Line` USING UTF8MB4) ELSE NULL END LineText
FROM defaultdb.SEARCHSTAGING
WHERE ID = @search_staging_id);
SET @jstable = CAST( @txt as JSON);

SELECT SearchId, 
`Name`, 
Zip, 
Address1, 
Address2, 
Address3, 
CaseNumber, 
DateFiled, 
Court, 
CaseType, 
CaseStyle, 
FirstName, 
LastName, 
Plantiff, 
`Status`
  FROM (

SELECT search_index SearchId,
	CASE 
    WHEN STR_TO_DATE( get_ddi.DateFiled, "%m/%d/%Y") IS NULL THEN '1900-01-01'
    ELSE STR_TO_DATE( get_ddi.DateFiled, "%m/%d/%Y") END
    DtFiled,
	get_ddi.*
FROM
JSON_TABLE(@jstable, '$[*]' COLUMNS (
                `Name` VARCHAR(150)  PATH '$.Name',
                `Zip` VARCHAR(150)  PATH '$.Zip',
                `Address1` VARCHAR(150)  PATH '$.Address1',
                `Address2` VARCHAR(150)  PATH '$.Address2',
                `Address3` VARCHAR(150)  PATH '$.Address3',
                `CaseNumber` VARCHAR(150)  PATH '$.CaseNumber',
                `DateFiled` VARCHAR(150)  PATH '$.DateFiled', -- this field is blank
                `Court` VARCHAR(150)  PATH '$.Court', -- this is potentially incorrect, always same value
                `CaseType` VARCHAR(150)  PATH '$.CaseType', -- case type is actually showing the date-filed
                `CaseStyle` VARCHAR(150)  PATH '$.CaseStyle', -- this item is always the first case in dataset
                `FirstName` VARCHAR(150)  PATH '$.FirstName',
                `LastName` VARCHAR(150)  PATH '$.LastName',
                `Plantiff` VARCHAR(150)  PATH '$.Plantiff',
                `Status` VARCHAR(150)  PATH '$.Status' -- this field is blank
                )
     ) get_ddi
     ) a
     ORDER BY DtFiled, CaseNumber;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_SEARCH_RESTRICTION_PARAMETERS
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_SEARCH_RESTRICTION_PARAMETERS`( IN user_index CHAR(36) )
BEGIN
-- Can user execute query
SET @islocked = TRUE;
SET @lockedReason = '';
SET @per_month = 0;
SET @per_year = 0;
SET @permissionName = 'User.Locked';
SET @maxPerMonthName = 'Setting.MaxRecords.Per.Month';
SET @maxPerYearName = 'Setting.MaxRecords.Per.Year';
SET @islocked = (SELECT KeyValue = 'Locked'
FROM USERPERMISSION P
JOIN PERMISSIONMAP M
  ON P.PermissionMapId = M.Id
WHERE 1 = 1
  AND M.KeyName = @permissionName
  AND P.UserId = user_index);
SET @lockedReason = CASE WHEN @islocked = TRUE THEN 'Account Is locked.' ELSE '' END;
DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_key_values;
DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_searched;

CREATE TEMPORARY TABLE tb_tmp_user_searched AS
SELECT *
  FROM USERDOWNLOADHISTORY
  WHERE 1 = 1
    AND UserId = user_index
    AND PurchaseDate IS NOT NULL
    AND YEAR(PurchaseDate) = YEAR( utc_timestamp() );

CREATE TEMPORARY TABLE tb_tmp_user_key_values AS
SELECT 
	M.KeyName,
    CASE WHEN ceil(KeyValue) = KeyValue THEN CAST( KeyValue AS UNSIGNED) ELSE 0 END KeyValue
  FROM USERPERMISSION P
  INNER JOIN PERMISSIONMAP M
  ON P.PermissionMapId = M.Id
  WHERE P.UserId = user_index
    AND M.KeyName IN 
    (
    @maxPerMonthName,
    @maxPerYearName
    );
SET @per_month = (SELECT KeyValue FROM tb_tmp_user_key_values WHERE KeyName = @maxPerMonthName);
SET @per_month = CASE WHEN @per_month IS NULL THEN 0 ELSE @per_month END;
SET @per_year = (SELECT KeyValue FROM tb_tmp_user_key_values WHERE KeyName = @maxPerYearName);
SET @per_year = CASE WHEN @per_year IS NULL THEN 0 ELSE @per_year END;
SET @searched_this_month = (SELECT SUM(RowCount) FROM tb_tmp_user_searched WHERE MONTH(PurchaseDate) = MONTH( utc_timestamp() ));
SET @searched_this_month = CASE WHEN @searched_this_month IS NULL THEN 0 ELSE @searched_this_month END;
SET @searched_this_year  = (SELECT SUM(RowCount) FROM tb_tmp_user_searched);
SET @searched_this_year = CASE WHEN @searched_this_year IS NULL THEN 0 ELSE @searched_this_year END;

DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_key_values;
DROP TEMPORARY TABLE IF EXISTS tb_tmp_user_searched;

SELECT 
	@islocked IsLocked,
    @lockedReason Reason,
    @per_month MaxPerMonth,
    @searched_this_month ThisMonth,
    @per_year MaxPerYear,
    @searched_this_year ThisYear;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_GET_SEARCH_USER_INDEX
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_GET_SEARCH_USER_INDEX`( IN search_index VARCHAR(50) )
BEGIN
	SELECT UserId
      FROM SEARCH
      WHERE Id = search_index;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_INSERT_PAYMENT_SESSION
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_INSERT_PAYMENT_SESSION`(
in idx CHAR(36),
in user_id CHAR(36),
in session_type VARCHAR(15),
in session_id VARCHAR(75),
in intent_id VARCHAR(50),
in client_id VARCHAR(75),
in external_id VARCHAR(50),
in js_text mediumblob
)
BEGIN
	set @invoice_id = (SELECT ReferenceId FROM USERINVOICE WHERE ExternalId = external_id LIMIT 1);
    INSERT PAYMENTSESSION
    (
		`Id`, `UserId`, `InvoiceId`, `SessionType`, 
        `SessionId`, `IntentId`, `ClientId`, `ExternalId`,
		JsText
    )
    SELECT
		idx `Id`, 
        user_id `UserId`, 
        @invoice_id `InvoiceId`, 
        session_type `SessionType`, 
        session_id `SessionId`, 
        intent_id `IntentId`, 
        client_id `ClientId`, 
        external_id `ExternalId`,
        js_text JsText
        WHERE @invoice_id IS NOT NULL;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_INVOICE_SET_EXTERNALID_AND_PAYMENT_DATE
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_INVOICE_SET_EXTERNALID_AND_PAYMENT_DATE`()
BEGIN
DROP TEMPORARY TABLE IF EXISTS tm_inv_lookup;
CREATE TEMPORARY TABLE tm_inv_lookup
SELECT
	ReferenceId,
    ActualId,
    PurchaseDate
FROM
(
SELECT
	des.ReferenceId, 
    des.ExternalId,
    src.ExternalId ActualId,
    src.PurchaseDate
    FROM USERINVOICE des
    JOIN (
		SELECT ReferenceId, ExternalId, PurchaseDate 
        FROM USERINVOICE
        WHERE LineId = 0
    ) src
    ON des.ReferenceId = src.ReferenceId
    WHERE des.LineId != 0
      AND (
		src.ExternalId != des.ExternalId
        OR des.ExternalId is null)
	) q
    GROUP BY ReferenceId, ActualId, PurchaseDate;
    
    UPDATE USERINVOICE I JOIN tm_inv_lookup T
      ON I.ReferenceId = T.ReferenceId
      SET 
      I.ExternalId = T.ActualId,
      I.PurchaseDate = T.PurchaseDate
      WHERE I.Id != ''
        AND I.LineId > 0;
	
    DROP TEMPORARY TABLE IF EXISTS tm_inv_lookup;
      
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_INVOICE_IS_DOWNLOADED
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_INVOICE_IS_DOWNLOADED`(in search_index char(36))
BEGIN

	SET @icount = (
	SELECT COUNT( 1 ) Nn
	  FROM  USERDOWNLOADHISTORY ui
	  WHERE SearchId = search_index
        AND AllowRollback = FALSE
        AND RollbackCount <= 5
		AND PurchaseDate is not null);
SELECT CASE 
		WHEN @icount is null THEN FALSE
		WHEN @icount = 0 THEN TRUE
        ELSE FALSE END IsDownloaded
WHERE EXISTS( SELECT 1 FROM USERDOWNLOADHISTORY WHERE SearchId = search_index AND PurchaseDate is not null);
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_INVOICE_IS_PAID
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_INVOICE_IS_PAID`(in search_index char(36))
BEGIN

CALL USP_CREATE_SESSION_FOR_ZERO_INVOICES();

SET @icount = (
SELECT COUNT( 1 ) Nn
  FROM USERINVOICE ui
  WHERE ReferenceId = search_index
    AND PurchaseDate is null);

SELECT CASE 
		WHEN @icount is null THEN FALSE
		WHEN @icount = 0 THEN TRUE
        ELSE FALSE END IsPaid
WHERE EXISTS( SELECT 1 FROM USERINVOICE WHERE ReferenceId = search_index);
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_INVOICE_IS_PAID_AND_DOWNLOADED
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_INVOICE_IS_PAID_AND_DOWNLOADED`(in search_index char(36))
BEGIN

SET @icount = (
SELECT COUNT( 1 ) Nn
  FROM USERINVOICE ui
  WHERE ReferenceId = search_index
    AND PurchaseDate is null);
		
SET @dcount = (
SELECT COUNT( 1 ) Nn
  FROM  USERDOWNLOADHISTORY ui
  WHERE SearchId = search_index
	AND AllowRollback = FALSE
	AND RollbackCount <= 5
	AND PurchaseDate is not null);

set @is_downloaded = (SELECT CASE 
		WHEN @dcount is null THEN FALSE
		WHEN @dcount > 0 THEN TRUE
        ELSE FALSE END IsDownloaded
WHERE EXISTS( SELECT 1 FROM USERDOWNLOADHISTORY WHERE SearchId = search_index AND PurchaseDate is not null));

set @is_paid = (SELECT CASE 
		WHEN @icount is null THEN FALSE
		WHEN @icount = 0 THEN TRUE
        ELSE FALSE END IsPaid
WHERE EXISTS( SELECT 1 FROM USERINVOICE WHERE ReferenceId = search_index));

SELECT @is_paid IsPaid, @is_downloaded IsDownloaded;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_IS_VALID_EXTERNAL_INDEX
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_IS_VALID_EXTERNAL_INDEX`(IN external_index VARCHAR(50))
BEGIN
	SET @isFound = (
	SELECT TRUE
	FROM
	(
	SELECT DISTINCT ExternalId
	FROM USERINVOICE
	WHERE ExternalId IS NOT NULL
	  AND IsDeleted = FALSE ) a
	WHERE a.ExternalId = external_index
	LIMIT 1);
	SET @isFound = CASE WHEN @isFound IS NULL THEN FALSE ELSE TRUE END;
	SELECT @isFound IsFound;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_QUERY_USER_PURCHASE_HISTORY
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_USER_PURCHASE_HISTORY`(IN user_index CHAR(36))
BEGIN
	DROP TEMPORARY TABLE IF EXISTS tmp_user_purchases;
	CREATE TEMPORARY TABLE tmp_user_purchases
	SELECT 
		CONVERT(sr.Line USING UTF8MB4) LineJs,
		ui.PurchaseDate,
		ui.ReferenceId,
		ui.ExternalId,
		'SEARCH :' ItemType,
		ui.ItemCount,
		( SELECT SUM( Price ) FROM USERINVOICE s  WHERE s.ReferenceId = ui.ReferenceId ) Price,
		CASE 
			WHEN hh.Id IS NULL THEN 'Purchased'
			WHEN hh.Id IS NOT NULL AND hh.PurchaseDate IS NOT NULL THEN 'Downloaded'
			ELSE 'Purchased' END StatusText
	  FROM USERINVOICE ui
	  LEFT
	  JOIN USERDOWNLOADHISTORY hh
		ON ui.ReferenceId = hh.SearchId
	  LEFT 
	  JOIN SEARCHREQUEST sr
		ON ui.ReferenceId = sr.SearchId
	  WHERE ui.PurchaseDate IS NOT NULL
		AND ui.LineId = 0
		AND ui.UserId = user_index
	  ORDER BY ui.CreateDate;
	
	SELECT
		PurchaseDate, 
		ReferenceId, 
		ExternalId, 
		concat( ItemType, ' ', CountyName, ', ', StateName, ' from ', StartingDate, ' to ', EndingDate) ItemType,
		ItemCount, 
		Price, 
		StatusText
	FROM
	(  
	SELECT
		UPPER( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.state') ) ) StateName,
		UPPER( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.county.name') ) ) CountyName,
		CAST( from_unixtime(floor( CAST(( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.end') ) ) AS UNSIGNED) /1000)) AS DATE )EndingDate,
		CAST( from_unixtime(floor( CAST(( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.start') ) ) AS UNSIGNED) /1000)) AS DATE ) StartingDate,
		t.*
	FROM tmp_user_purchases t
	) q;
	DROP TEMPORARY TABLE IF EXISTS tmp_user_purchases;
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
		Id, 
		UserId, 
		ExpectedRows, 
		CreateDate, 
        CASE WHEN SearchProgress != "3 - Completed" THEN SearchProgress
			 WHEN SearchProgress = 3 
			 AND EXISTS ( SELECT 1 FROM USERINVOICE WHERE ReferenceId = b.Id AND PurchaseDate IS NOT NULL AND IsDeleted = FALSE)
			 THEN "4 - Purchased"
			 WHEN SearchProgress = 3 
			 AND EXISTS ( SELECT 1 FROM USERINVOICE WHERE ReferenceId = b.Id AND PurchaseDate IS NOT NULL AND IsDeleted = FALSE)
			 AND EXISTS ( SELECT 1 FROM USERDOWNLOADHISTORY WHERE SearchId = b.Id)
			 THEN "5 - Downloaded"
			 ELSE SearchProgress END SearchProgress,
        StateCode,
		CountyName,
		EndDate,
		StartDate
FROM (
	SELECT 
		Id, 
		UserId, 
		ExpectedRows, 
		CreateDate, 
		SearchProgress,
		CASE WHEN LineJs IS NULL THEN 'N/A'
			ELSE UPPER( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.state') ) ) END StateCode,
		CASE WHEN LineJs IS NULL THEN 'N/A'
			ELSE UPPER( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.county.name') ) ) END CountyName,
		CASE WHEN LineJs IS NULL THEN NULL
			ELSE from_unixtime(floor( CAST(( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.end') ) ) AS UNSIGNED) /1000)) END EndDate,
		CASE WHEN LineJs IS NULL THEN NULL
			ELSE from_unixtime(floor( CAST(( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.start') ) ) AS UNSIGNED) /1000)) END StartDate
		FROM (
		SELECT 
		S.Id,
		S.UserId,
		S.ExpectedRows, 
		S.CreateDate,
			CASE 
				WHEN EndDate is null AND ExpectedRows is NULL THEN "1 - Submitted"
				WHEN EndDate is null AND ExpectedRows is not NULL THEN "2 - Processing"
				WHEN EndDate is not null AND ExpectedRows is not NULL AND ExpectedRows > 0 THEN "3 - Completed"
				ELSE "10 - Error" END SearchProgress,
		  CAST( CONVERT(`Line` USING UTF8MB4)  AS JSON ) `LineJs`
		  FROM SEARCH S
		  LEFT JOIN SEARCHREQUEST R ON S.Id = R.SearchId AND 1 = LineNbr
		  WHERE UserId = userUid
		) a
	) b
	ORDER BY b.CreateDate DESC;
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
-- procedure USP_QUERY_USER_WITHOUT_CUSTOMER_ACCOUNT
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_QUERY_USER_WITHOUT_CUSTOMER_ACCOUNT`(IN account_type VARCHAR(50))
BEGIN
SET @is_test = CASE WHEN @account_type = 'test' THEN TRUE ELSE FALSE END;
SELECT uu.Id, uu.UserName, uu.Email
  FROM USERS uu
  LEFT JOIN PAYMENTCUSTOMER pc
  ON uu.Id = pc.UserId
  WHERE pc.Id IS NULL
    AND uu.Id != '00000000-0000-0000-0000-000000000000'
    AND uu.UserName != 'bad-account-invalid';
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_SEARCH_DOWNLOAD_ROLLBACK
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SEARCH_DOWNLOAD_ROLLBACK`(in search_index char(36), in user_index char(36))
BEGIN
	IF EXISTS ( SELECT 1 FROM USERDOWNLOADHISTORY 
		WHERE UserId = user_index 
        AND SearchId = search_index 
        AND RollbackCount < 5) THEN
        
        UPDATE 		USERDOWNLOADHISTORY
			SET		AllowRollback = TRUE,
					RollbackCount = RollbackCount + 1
			WHERE	UserId = user_index
              AND 	SearchId = search_index;
              
	END IF;

	SELECT 	Id, UserId, SearchId, Price, RowCount, 
			InvoiceId, PurchaseDate, AllowRollback, RollbackCount, CreateDate
		FROM USERDOWNLOADHISTORY 
		WHERE UserId = user_index 
        AND SearchId = search_index;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_SEARCH_DOWNLOAD_ROLLBACK_COMPLETED
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SEARCH_DOWNLOAD_ROLLBACK_COMPLETED`(in search_index char(36))
BEGIN
	UPDATE 		USERDOWNLOADHISTORY
		SET		AllowRollback = FALSE
		WHERE	SearchId = search_index
          AND	AllowRollback = TRUE;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_SEARCH_INVOICE_SET_EXTERNAL_ID
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SEARCH_INVOICE_SET_EXTERNAL_ID`(
	IN uu_index CHAR(36), 
	IN search_index CHAR(36)
)
BEGIN
IF EXISTS ( SELECT 1 FROM USERINVOICE WHERE ReferenceId = search_index AND UserId = uu_index AND IsDeleted = false ) THEN
	SET @external_id = (
		SELECT ExternalId
		  FROM USERINVOICE
		  WHERE UserId = @uu_index
			AND LineId = 0
			AND ReferenceId = search_index
			AND IsDeleted = false
			LIMIT 1);
		
	UPDATE USERINVOICE
		SET	
		ExternalId = @external_id
		WHERE 1 = 1
		AND @external_id IS NOT NULL
		AND UserId = @uu_index
		AND ReferenceId = search_index
		AND LineId <> 0
		AND ExternalId IS NULL
		AND IsDeleted = false;
	
END IF;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_SEARCH_QUEUE_RETRY_FAILED_REQUEST
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SEARCH_QUEUE_RETRY_FAILED_REQUEST`()
BEGIN

IF EXISTS( SELECT 1 FROM SEARCH WHERE ExpectedRows IS NOT NULL AND EndDate IS NULL ) THEN

	SET @nw = utc_timestamp();
	DROP TEMPORARY TABLE IF EXISTS tb_tmp_search_timeout;
	CREATE TEMPORARY TABLE tb_tmp_search_timeout AS
	SELECT 
		TIMESTAMPDIFF(MINUTE, S.StartDate, @nw)  AgeInMinutes,
		S.*
	  FROM SEARCH S
	  WHERE ExpectedRows IS NOT NULL
		AND EndDate IS NULL
		AND TIMESTAMPDIFF(MINUTE, S.StartDate, @nw) > 
			CASE 
				WHEN RetryCount = 0 THEN 30
				WHEN RetryCount = 1 THEN 60
				WHEN RetryCount = 2 THEN 120
				WHEN RetryCount = 3 THEN 240
				WHEN RetryCount = 4 THEN 480
				ELSE 30 END;
		
	UPDATE SEARCH S JOIN tb_tmp_search_timeout T on S.Id = T.Id
	SET
		S.StartDate = @nw,
		S.ExpectedRows = NULL,
		S.RetryCount = S.RetryCount + 1
	WHERE S.RetryCount < 6;
		
	DROP TEMPORARY TABLE IF EXISTS tb_tmp_search_timeout;

END IF;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_SEARCH_SET_ESTIMATED_ROW_COUNT
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SEARCH_SET_ESTIMATED_ROW_COUNT`()
BEGIN
DECLARE staged_record_count VARCHAR(75) DEFAULT 'data-record-count';

UPDATE SEARCH S
	JOIN SEARCHSTAGING Ss ON Ss.SearchId = S.Id
    SET
		S.ExpectedRows = CAST( (CONVERT(`Line` USING UTF8MB4)) AS UNSIGNED )	
  WHERE 1 = 1
    AND S.Id != ''
	AND Ss.IsBinary = 0
    AND Ss.StagingType = staged_record_count
    AND CASE WHEN S.ExpectedRows IS NULL THEN 0 ELSE S.ExpectedRows END = 0;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_SEARCH_SET_FINAL_ROW_COUNT
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SEARCH_SET_FINAL_ROW_COUNT`()
BEGIN
	DECLARE search_key VARCHAR(75) DEFAULT 'data-output-person-addres';
    -- build temp table
	DROP TEMPORARY TABLE IF EXISTS TMPSEARCHSTAGINGJS;
	CREATE TEMPORARY TABLE TMPSEARCHSTAGINGJS
		SELECT
			a.Id,
			a.SearchId,
			JSON_LENGTH( CAST( LineText AS json ) ) ActualRows
		FROM
		(
			SELECT 
				S.Id, 
				S.SearchId,
				CASE WHEN IsBinary = 0 THEN CONVERT(`Line` USING UTF8MB4) ELSE NULL END LineText
			FROM SEARCHSTAGING S
			JOIN (
				SELECT SearchId, MAX( LineNbr ) LineNbr
				FROM SEARCHSTAGING
				WHERE StagingType LIKE 'data-output-person-addres%'
				GROUP BY SearchId ) Sq
			ON 	S.SearchId = Sq.SearchId
			AND S.LineNbr = Sq.LineNbr
		) a
		WHERE LineText IS NOT NULL;
    
	-- set estimated rows = to actual rows
	UPDATE SEARCH S
	  JOIN TMPSEARCHSTAGINGJS Ss
		ON S.Id = Ss.SearchId
	   SET 
       S.ExpectedRows = Ss.ActualRows,
       S.EndDate = CASE WHEN S.EndDate IS NULL THEN utc_timestamp() ELSE S.EndDate END
	 WHERE 1 = 1
		AND S.Id != ''
		AND 
        CASE WHEN S.ExpectedRows IS NULL = 0 THEN 0 ELSE S.ExpectedRows END != 
		Ss.ActualRows;
	-- clean up temp table
	DROP TEMPORARY TABLE IF EXISTS TMPSEARCHSTAGINGJS;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_SET_ACTIVE_PRICING_TEMPLATE
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SET_ACTIVE_PRICING_TEMPLATE`(
	IN permission_code_index VARCHAR(50),
    IN pricing_json VARCHAR(1000))
BEGIN
SET @id = permission_code_index; 
SET @my_json = pricing_json;
SET @currentDate = DATE_ADD(utc_timestamp(), INTERVAL 1 second);
SET @permission_group_index = (
SELECT PermissionGroupId
  FROM PERMISSIONGROUPCODES pgc
  WHERE pgc.Id = permission_code_index);

	INSERT INTO `defaultdb`.`PERMISSIONGROUPCODES`
	(
	`PermissionGroupId`,
	`KeyName`,
	`ProductCode`,
	`PriceCodeAnnual`,
	`PriceCodeMonthly`,
	`KeyJs`,
	`IsActive`,
    CreateDate
	)
SELECT
		js.`PermissionGroupId`,
		pg.`KeyName`,
		js.`ProductCode`,
		js.`PriceCodeAnnual`,
		js.`PriceCodeMonthly`,
		@my_json KeyJs,
		TRUE IsActive,
        @currentDate CurDate
	FROM (
		SELECT @permission_group_index PermissionGroupId,
		JSON_UNQUOTE( JSON_EXTRACT( @my_json, '$.product.code' ) ) ProductCode,
		JSON_UNQUOTE( JSON_EXTRACT( @my_json, '$.pricecode.annual' ) ) PriceCodeAnnual,
		JSON_UNQUOTE( JSON_EXTRACT( @my_json, '$.pricecode.monthly' ) ) PriceCodeMonthly
		) js
	JOIN ( SELECT * FROM PERMISSIONGROUPCODES WHERE Id = @id LIMIT 1 ) pg
	ON	js.PermissionGroupId = pg.PermissionGroupId
	WHERE	1 = 1
	  AND	js.PermissionGroupId IS NOT NULL
	  AND	js.ProductCode IS NOT NULL
	  AND	js.PriceCodeAnnual IS NOT NULL
	  AND	js.PriceCodeMonthly IS NOT NULL;
      
	SET @pgc_id = (SELECT Id FROM PERMISSIONGROUPCODES WHERE CreateDate = @currentDate ORDER BY CreateDate LIMIT 1);
    IF @pgc_id IS NOT NULL THEN
		SET @key_name = (SELECT KeyName FROM PERMISSIONGROUPCODES WHERE Id = @pgc_id);
        UPDATE PERMISSIONGROUPCODES
        SET IsActive = FALSE
        WHERE
        IsActive = TRUE
        AND KeyName = @key_name
        AND Id != @pgc_id;
    END IF;
    
	SELECT *
    FROM PERMISSIONGROUPCODES
    WHERE Id = @pgc_id;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_SET_INVOICE_PURCHASE_DATE
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SET_INVOICE_PURCHASE_DATE`(IN external_index VARCHAR(50))
BEGIN
	
UPDATE USERINVOICE Ii
SET
	Ii.PurchaseDate = utc_timestamp()
	WHERE Ii.ExternalId = external_index
	  AND external_index IS NOT NULL
	  AND Ii.IsDeleted = FALSE
	  AND Ii.PurchaseDate IS NULL
	  AND Ii.Id != '';

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_UPDATE_DISCOUNTREQUEST
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_UPDATE_DISCOUNTREQUEST`(IN pay_load mediumtext)
BEGIN
SET @payload = CAST( pay_load as JSON);

DROP TEMPORARY TABLE IF EXISTS tmp_js_level_update;
CREATE TEMPORARY TABLE tmp_js_level_update
SELECT 
    js.ExternalId, 
    js.IsPaymentSuccess, 
    utc_timestamp() CompletionDate
FROM (
	SELECT 
	JSON_VALUE( @payload, '$.IsPaymentSuccess') IsPaymentSuccess
    , JSON_VALUE( @payload, '$.ExternalId') ExternalId ) js
WHERE js.IsPaymentSuccess IS NOT NULL
  AND js.ExternalId IS NOT NULL
  AND EXISTS( SELECT 1 FROM DISCOUNTREQUEST R WHERE R.ExternalId = js.ExternalId )
  LIMIT 1;
  
IF EXISTS( SELECT 1 FROM tmp_js_level_update ) THEN
	UPDATE DISCOUNTREQUEST R
    JOIN tmp_js_level_update U
    ON R.ExternalId = U.ExternalId
    SET
		R.IsPaymentSuccess = U.IsPaymentSuccess,
        R.CompletionDate = U.CompletionDate;
END IF;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure USP_UPDATE_LEVELREQUEST
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_UPDATE_LEVELREQUEST`(IN pay_load mediumtext)
BEGIN
SET @payload = CAST( pay_load as JSON);

DROP TEMPORARY TABLE IF EXISTS tmp_js_level_update;
CREATE TEMPORARY TABLE tmp_js_level_update
SELECT 
    js.ExternalId, 
    js.IsPaymentSuccess, 
    utc_timestamp() CompletionDate
FROM (
	SELECT 
	JSON_VALUE( @payload, '$.IsPaymentSuccess') IsPaymentSuccess
    , JSON_VALUE( @payload, '$.ExternalId') ExternalId ) js
WHERE js.IsPaymentSuccess IS NOT NULL
  AND js.ExternalId IS NOT NULL
  AND EXISTS( SELECT 1 FROM LEVELREQUEST R WHERE R.ExternalId = js.ExternalId )
  LIMIT 1;
  
IF EXISTS( SELECT 1 FROM tmp_js_level_update ) THEN
	UPDATE LEVELREQUEST R
    JOIN tmp_js_level_update U
    ON R.ExternalId = U.ExternalId
    SET
		R.IsPaymentSuccess = U.IsPaymentSuccess,
        R.CompletionDate = U.CompletionDate;
END IF;
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
-- procedure USP_USER_APPEND_LOCK_PERMISSION
-- -----------------------------------------------------

DELIMITER $$
USE `defaultdb`$$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_USER_APPEND_LOCK_PERMISSION`()
BEGIN
	INSERT USERPERMISSION
	( Id, UserId, PermissionMapId, KeyValue )
	SELECT
		concat( LEFT(id,8), "-", MID(Id,8,4), "-", MID(Id, 12,4), "-", MID(Id, 16,4), "-", RIGHT(Id,12)) Id,
		UserId,
		PermissionMapId,
		KeyValue
	FROM
	(
	SELECT 
		md5(UUID()) Id,
		U.Id UserId,
		@permissionId PermissionMapId,
		'' KeyValue
	FROM USERS U
	LEFT JOIN USERPERMISSION Up
	ON U.Id = Up.UserId
	AND @permissionId = Up.PermissionMapId
	WHERE Up.Id IS NULL
	) SQ;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- View `defaultdb`.`VWUSERPERMISSION`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `defaultdb`.`VWUSERPERMISSION`;
USE `defaultdb`;
CREATE  OR REPLACE ALGORITHM=UNDEFINED DEFINER=`app-dev-account`@`%` SQL SECURITY DEFINER VIEW `defaultdb`.`VWUSERPERMISSION` AS select `u`.`Id` AS `id`,`u`.`UserId` AS `userid`,`u`.`PermissionMapId` AS `permissionmapid`,`u`.`KeyValue` AS `keyvalue`,`p`.`KeyName` AS `keyname`,`p`.`OrderId` AS `orderid` from (`defaultdb`.`USERPERMISSION` `u` join `defaultdb`.`PERMISSIONMAP` `p` on((`u`.`PermissionMapId` = `p`.`Id`)));

-- -----------------------------------------------------
-- View `defaultdb`.`VWUSERPROFILE`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `defaultdb`.`VWUSERPROFILE`;
USE `defaultdb`;
CREATE  OR REPLACE ALGORITHM=UNDEFINED DEFINER=`app-dev-account`@`%` SQL SECURITY DEFINER VIEW `defaultdb`.`VWUSERPROFILE` AS select `u`.`Id` AS `id`,`u`.`UserId` AS `userid`,`u`.`ProfileMapId` AS `profilemapid`,`u`.`KeyValue` AS `keyvalue`,`p`.`KeyName` AS `keyname`,`p`.`OrderId` AS `orderid` from (`defaultdb`.`USERPROFILE` `u` join `defaultdb`.`PROFILEMAP` `p` on((`u`.`ProfileMapId` = `p`.`Id`)));

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
