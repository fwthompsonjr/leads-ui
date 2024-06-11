/*
	as a web-service
    i want to fetch the count of email messages for a specific user
	email-get-message-count
    design-note
    api method is post
    require authorization header
    require body { "Id": "MessageId" }
*/

use wlogpermissions;
set @user_index = (
SELECT UserId
FROM (
SELECT UserId
FROM CORRESPONDENCE
GROUP BY UserId
) uu
ORDER BY RAND()
LIMIT 1);
drop temporary table if exists tmp_user_correspondence_filter;
drop temporary table if exists tmp_user_correspondence;

create temporary table tmp_user_correspondence
SELECT 
row_number() over ( order by CreateDate DESC) RowId,
 DATE_FORMAT(`CreateDate`, '%Y-%m-%d %H:%i') CreateDt,
 c.*
 FROM CORRESPONDENCE c
 WHERE UserId = @user_index
 ORDER BY CreateDate DESC;
 
 create temporary table tmp_user_correspondence_filter
	SELECT max( RowId ) RowId, CreateDt, `Subject`
    FROM tmp_user_correspondence
    GROUP BY CreateDt, `Subject`;
    
set @item_count = (
	SELECT COUNT( 1 )
    FROM tmp_user_correspondence uc
    JOIN tmp_user_correspondence_filter ucf 
    ON uc.RowId = ucf.RowId);
    
SELECT
	UUID() Id,
	@user_index UserId, 
    @item_count Items
  
  