
DELIMITER $$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_SET_CURRENT_REVISION`( IN contentReferenceId char(36), versionNumber INT )
BEGIN
	SET @recordIndex = (SELECT InternalId FROM CONTENT WHERE Id = contentReferenceId);
    -- set active flag = true for current selection
    UPDATE CONTENT
    SET
	IsActive = true
    WHERE 	Id = contentReferenceId
	AND 	InternalId = @recordIndex
	AND 	VersionId = versionNumber
	AND 	EXISTS (SELECT 1 FROM CONTENTLINE WHERE versionNumber = VersionId);
    
	
    -- set active flag
    UPDATE CONTENT
    SET
	IsActive = false
    WHERE 	Id != contentReferenceId
	AND 	InternalId = @recordIndex
	AND 	VersionId = versionNumber
	AND 	EXISTS (SELECT 1 FROM CONTENTLINE WHERE versionNumber = VersionId);
END$$
DELIMITER ;