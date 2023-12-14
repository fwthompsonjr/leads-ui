DELIMITER $$
CREATE PROCEDURE `USP_INITIALIZE_CONTENT_LINE`(
	IN parmContentName varchar(500),
    IN lineNumber int,
    IN lineContent varchar(500)
    )
BEGIN
	DECLARE contactIndx CHAR(36);
	IF	EXISTS ( SELECT 1 FROM CONTENT WHERE ContentName = parmContentName ) THEN
		SET contactIndx = (SELECT MIN(Id) FROM CONTENT WHERE ContentName = parmContentName AND IsActive = true );        
        
        INSERT CONTENTLINE ( ContentId, InternalId, LineNbr, Content )
        SELECT 
			i.ContentId, i.InternalId, i.LineNbr, i.Content
        FROM (SELECT
				C.Id ContentId,
				C.InternalId,
				lineNumber LineNbr,
				lineContent Content
			FROM CONTENT C
			WHERE C.Id = contectIndx) i
		LEFT JOIN CONTENTLINE CL
        ON 		i.ContentId = CL.ContentId
        AND 	i.LineNbr = CL.LineNbr
        WHERE	CL.Id IS NULL;
        
	END IF;
END$$
DELIMITER ;
