set @uuidx = '00000000-0000-0000-0000-000000000000';
SET @zeroid = 'f98bd917-bcf6-4975-9b6e-1831f131bf47';

DELETE
  FROM SEARCH
  WHERE Id != @zeroid;
  
DELETE
  FROM SEARCHDETAIL
  WHERE Id != @uuidx;
DELETE
  FROM SEARCHREQUEST
  WHERE Id != @uuidx;
DELETE
  FROM SEARCHRESPONSE
  WHERE Id != @uuidx;
DELETE
  FROM SEARCHSTAGING
  WHERE Id != @uuidx;
DELETE
  FROM SEARCHSTATUS
  WHERE Id != @uuidx;