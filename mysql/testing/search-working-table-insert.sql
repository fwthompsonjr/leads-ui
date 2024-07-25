SET @js =
TRIM('
{
 "src": "LOCALHOST",
 "ids": [
	{ "id": "7b10201a-40b2-11ef-99ce-0af7a01f52e9" },
	{ "id": "7ac5a7c9-40b2-11ef-99ce-0af7a01f52e9" },
	{ "id": "01caaf9e-40b0-11ef-99ce-0af7a01f52e9" },
	{ "id": "0169cde5-40b0-11ef-99ce-0af7a01f52e9" },
	{ "id": "b20e6dfe-3c74-11ef-99ce-0af7a01f52e9" },
	{ "id": "512b03d7-3c06-11ef-99ce-0af7a01f52e9" },
	{ "id": "4f7cdde2-3c06-11ef-99ce-0af7a01f52e9" },
	{ "id": "4dadaaf9-3c06-11ef-99ce-0af7a01f52e9" },
	{ "id": "d6d2f121-3c04-11ef-99ce-0af7a01f52e9" },
	{ "id": "d6465dcc-3c04-11ef-99ce-0af7a01f52e9" },
	{ "id": "912d38ca-3c04-11ef-99ce-0af7a01f52e9" },
	{ "id": "865268fc-3c02-11ef-99ce-0af7a01f52e9" },
	{ "id": "7ecd00f3-3c01-11ef-99ce-0af7a01f52e9" },
	{ "id": "3a772d24-363d-11ef-99ce-0af7a01f52e9" },
	{ "id": "c07d1957-363c-11ef-99ce-0af7a01f52e9" }
 ]
}');
/*
	status type
    0 - begin
    1 - complete
    2 - failed
*/

drop temporary table if exists tmp_queue_start_requested;

SET @computerName = JSON_VALUE( @js, "$.src" );
create temporary table tmp_queue_start_requested
SELECT @computerName `Cpu`, `Index`, utc_timestamp() CreateDate
FROM
(
	SELECT ii.`Index`
	FROM JSON_TABLE( @js, "$.ids[*]" COLUMNS (`Index` VARCHAR(50) PATH "$.id" )) ii
	GROUP BY ii.`Index`
) i
LEFT JOIN SEARCH s
ON i.`Index` = s.Id
WHERE s.Id is not null;

UPDATE SEARCHWORKING w
  JOIN tmp_queue_start_requested t
    ON t.`Index` = w.SearchId
    SET
	MessageId = 0,
    StatusId = 0,
    MachineName = t.`Cpu`,
    CreateDate = t.CreateDate,
    CompletionDate = NULL
WHERE	w.Id != '';

INSERT SEARCHWORKING 
(
	SearchId, MessageId, StatusId, MachineName, CreateDate, CompletionDate
)
SELECT 
	t.`Index` SearchId, 
    0 MessageId, 
    0 StatusId, 
    t.`Cpu` MachineName, 
    t.CreateDate, 
    NULL CompletionDate
  FROM tmp_queue_start_requested t
  LEFT JOIN SEARCHWORKING w
  ON	t.`Index` = w.SearchId
  WHERE	w.Id is null;
