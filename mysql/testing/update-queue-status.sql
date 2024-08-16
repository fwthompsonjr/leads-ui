/*
logic check
*/

SET @js = '{
	"Message": "record retrieved from search query",
	"StatusId": 2,
	"Id": "000-000-000-00-00",
	"SearchId": "000-000-000-00-00"
}';    
drop temporary table if exists tmp_queue_status_header;
drop temporary table if exists tmp_queue_status_detail;
create temporary table tmp_queue_status_header (
	`Id` char(36) NULL,
	`SearchId` char(36) NULL,
	`Message` VARCHAR(255),
	`StatusId` int NOT NULL DEFAULT (0)
);

SET @work_id = JSON_UNQUOTE( JSON_EXTRACT( @js, '$.Id' ) );
SET @search_id = JSON_UNQUOTE( JSON_EXTRACT( @js, '$.SearchId' ) );
SET @status_id = CAST( JSON_UNQUOTE( JSON_EXTRACT( @js, '$.StatusId' ) ) AS SIGNED);
SET @mssg = JSON_UNQUOTE( JSON_EXTRACT( @js, '$.Message' ) );

insert tmp_queue_status_header (  `Id`, `SearchId`, `StatusId`, `Message` )
SELECT 
	@work_id `Id`,
    @search_id `SearchId`,
    @status_id `StatusId`,
    @mssg `Message`;

SELECT * FROM tmp_queue_status_header;

create temporary table tmp_queue_status_detail
SELECT d.Id, d.SearchId, d.StatusId, d.`Message`
FROM tmp_queue_status_header d
	LEFT 
    JOIN  QUEUEWORKING q
      ON d.SearchId = q.Id
	LEFT 
    JOIN SEARCH s
      ON d.SearchId = s.Id
    LEFT 
    JOIN SEARCHWORKINGSTATUS sw
      ON d.StatusId = sw.Id
	WHERE 1 = 1
      AND q.Id is not null
      AND s.Id is not null
      AND sw.Id is not null;
      

SELECT * FROM tmp_queue_status_detail;