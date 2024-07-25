/*


*/

SET @js = '{
 "src": "LOCALHOST",
 "id": "7b10201a-40b2-11ef-99ce-0af7a01f52e9",
 "messageId": 0,
 "statusId": 0
 }';
 SET @lastUpdate = utc_timestamp();
 SET @statusFailed = (SELECT Id FROM SEARCHWORKINGSTATUS WHERE `Message` = 'failed');
 SET @statusCompleted = (SELECT Id FROM SEARCHWORKINGSTATUS WHERE `Message` = 'complete');
 SET @messageCompleted = (SELECT Id FROM SEARCHWORKINGMESSAGE WHERE `Message` = 'process completed');
 
 SET @statusFailed = CASE WHEN @statusFailed IS NULL THEN 2 ELSE @statusFailed END;
 SET @statusCompleted = CASE WHEN @statusCompleted IS NULL THEN 1 ELSE @statusCompleted END;
 SET @messageCompleted = CASE WHEN @messageCompleted IS NULL THEN 6 ELSE @messageCompleted END;
 
DROP TEMPORARY TABLE IF EXISTS tmp_working_parms;
CREATE TEMPORARY TABLE tmp_working_parms
SELECT
	SearchId, MessageId, StatusId, MachineName,
    CASE 
    WHEN StatusId = @statusCompleted AND MessageId = @messageCompleted THEN @lastUpdate 
    WHEN StatusId = @statusFailed THEN @lastUpdate 
    ELSE NULL END CompletionDate
FROM
(
SELECT 
	CAST( JSON_VALUE( @js, "$.id" ) AS CHAR(36)) SearchId,
	CONVERT( JSON_VALUE( @js, "$.messageId" ), SIGNED ) MessageId,
	CONVERT( JSON_VALUE( @js, "$.statusId" ), SIGNED ) StatusId,
	LEFT( TRIM( JSON_VALUE( @js, "$.src" ) ), 256 )  MachineName
) m;

UPDATE	SEARCHWORKING w
  JOIN	tmp_working_parms p
    ON 	w.SearchId = p.SearchId
   SET
   w.MessageId = p.MessageId,
   w.StatusId = p.StatusId,
   w.MachineName = p.MachineName,
   w.CompletionDate = p.CompletionDate,
   w.LastUpdateDt = @lastUpdate
 WHERE	w.Id != '';