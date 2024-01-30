set @uuidx = '00000000-0000-0000-0000-000000000000';

  
SELECT *
  FROM SEARCHDETAIL
  WHERE Id != @uuidx
  ORDER BY SearchId, LineNbr;
  
SELECT *
  FROM SEARCHREQUEST
  WHERE Id != @uuidx
  ORDER BY SearchId, LineNbr;
  
SELECT *
  FROM SEARCHRESPONSE
  WHERE Id != @uuidx
  ORDER BY SearchId, LineNbr;

SELECT *
  FROM SEARCHSTAGING
  WHERE Id != @uuidx
  ORDER BY SearchId, LineNbr;
  
SELECT *
  FROM SEARCHSTATUS
  WHERE Id != @uuidx
  ORDER BY SearchId, CreateDate, LineNbr;