DELIMITER $$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_INITIALIZE_CONTENT`(IN parmContentName varchar(500))
BEGIN
	IF NOT EXISTS ( SELECT 1 FROM CONTENT WHERE ContentName = parmContentName ) THEN
		CALL USP_INSERT_CONTENT( parmContentName );
	END IF;
END$$
DELIMITER ;