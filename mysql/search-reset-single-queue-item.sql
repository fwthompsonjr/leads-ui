use defaultdb;

/*
	Script:
    During the testing of api operation to perform search (unattended)
    It is needed to clear data from search tables
    from search				- reset EndDate and ExpectedRows to null
    from search-detail 		- don't change
    from search-request 	- don't change
    from search-response 	- don't change
    from search-staging		- remove all
    from search-status 		- remove all excluding line-number = 1
*/



SET @recid ='8f316534-bf75-11ee-be26-0af7a01f52e9';
SET @clear_item = TRUE;


UPDATE SEARCH
	SET EndDate  = NULL,
    ExpectedRows = NULL
WHERE Id = @recid
  AND @clear_item = TRUE;
  
DELETE
  FROM SEARCHSTAGING
  WHERE SearchId = @recid
  AND Id != ''
  AND @clear_item = TRUE;
  
DELETE
  FROM SEARCHSTATUS
  WHERE SearchId = @recid
  AND Id != ''
  AND LineNbr != 1
  AND @clear_item = TRUE;