DELIMITER $$
CREATE DEFINER=`admin`@`%` PROCEDURE `USP_CREATE_CONTENT_REVISION`( IN contentReferenceId char(36) )
BEGIN
	SET @currentVersionIndex = (SELECT MAX( VersionId ) FROM CONTENT WHERE Id = contentReferenceId);
	SET @internalIndex = (SELECT InternalId FROM CONTENT WHERE Id = contentReferenceId);
	SET @newVersionIndex = CASE WHEN @currentVersionIndex IS NULL THEN 0 ELSE @currentVersionIndex + 1 END;
    
    -- add new revision to CONTENT table
    INSERT INTO CONTENT
    (
		InternalId, VersionId, ContentName
    )
    SELECT InternalId, @newVersionIndex VersionId, ContentName
    FROM CONTENT
    WHERE Id = contentReferenceId
    AND VersionId = @currentVersionIndex;
	
    -- set / ensure is-child flag for child pages
    
	UPDATE CONTENT 
	  SET IsChild = true
	  WHERE ContentName in 
	  ( 
	  'HOME.PRE.LOGIN.060.TAB.1.WELCOME',
	  'HOME.PRE.LOGIN.060.TAB.2.LOGIN',
	  'HOME.PRE.LOGIN.060.TAB.3.REGISTER'
	  )
	  AND IsChild = false
	  AND Id != '25';
  
	SET @newIndex = (SELECT Id FROM CONTENT WHERE InternalId = @internalIndex AND VersionId = @newVersionIndex);
    
    
    -- add new revision to CONTENT table
    INSERT INTO CONTENTLINE
    (
		ContentId, InternalId, LineNbr, Content
    )
    SELECT @newIndex ContentId, @internalIndex InternalId, LineNbr, Content
    FROM CONTENTLINE
    WHERE ContentId = contentReferenceId
    AND InternalId = @internalIndex;
    
    
END$$
DELIMITER ;
