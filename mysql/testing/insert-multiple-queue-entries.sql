/*
	load items in queue to table
*/
SET @js = '{
	"Message": "record retrieved from search query",
	"StatusId": -1,
	"MachineName": "mycomputer",
	"Items": [
		{
			"Id": "00000000-0000-0000-0000-000000000000"
		}
	]
}';
drop temporary table if exists tmp_queue_header;
drop temporary table if exists tmp_queue_detail;

create temporary table tmp_queue_header (
  `Message` VARCHAR(255),
  `StatusId` int NOT NULL DEFAULT (0),
  `MachineName` varchar(256) DEFAULT NULL,
  `CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
  `LastUpdateDt` datetime NOT NULL DEFAULT (utc_timestamp())
);
create temporary table tmp_queue_detail (
	`SearchId` char(36) NULL,
	`Message` VARCHAR(255),
	`StatusId` int NOT NULL DEFAULT (0),
	`MachineName` varchar(256) DEFAULT NULL,
	`CreateDate` datetime NOT NULL DEFAULT (utc_timestamp()),
	`LastUpdateDt` datetime NOT NULL DEFAULT (utc_timestamp()),
	`CompletionDate` datetime DEFAULT NULL
);

SET @mssg = JSON_UNQUOTE( JSON_EXTRACT( @js, '$.Message' ) );
SET @status_id = CAST( JSON_UNQUOTE( JSON_EXTRACT( @js, '$.StatusId' ) ) AS SIGNED);
SET @machine_name = JSON_UNQUOTE( JSON_EXTRACT( @js, '$.MachineName' ) );
insert tmp_queue_header (  `Message`, `StatusId`, `MachineName` )
SELECT 
	@mssg `Message`,
    @status_id `StatusId`,
    @machine_name `MachineName`;

insert tmp_queue_detail (  
	SearchId, Message, StatusId, MachineName, CreateDate, LastUpdateDt, CompletionDate
 )
SELECT SearchId, Message, StatusId, MachineName, CreateDate, LastUpdateDt,
CASE WHEN StatusId IN ( 1, 2 ) THEN utc_timestamp() ELSE NULL END
FROM
JSON_TABLE(
    @js,
    '$.Items[*]' COLUMNS( SearchId VARCHAR(36) PATH '$.Id' NULL ON ERROR )
    ) as jt
CROSS JOIN tmp_queue_header h;

SELECT * FROM tmp_queue_detail;

SELECT 
d.SearchId, d.Message, d.StatusId, d.MachineName, d.CreateDate, d.LastUpdateDt, d.CompletionDate 
	FROM tmp_queue_detail d
    LEFT 
    JOIN SEARCH s
      ON d.SearchId = s.Id
    LEFT 
    JOIN SEARCHWORKINGSTATUS sw
      ON d.StatusId = sw.Id
	WHERE 1 = 1
      AND s.Id is not null
      AND sw.Id is not null;
      
drop temporary table if exists tmp_queue_header;
drop temporary table if exists tmp_queue_detail;